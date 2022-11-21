using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Extensions;
using MyWebApp.Models;
using MyWebApp.ViewModels;

namespace MyWebApp.Repository
{
    public sealed class NotesRepository
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ApplicationDbContext _dbContext;
        private readonly PicturesLoader _picturesLoader;
        private readonly RandomGenerator _randomGenerator;

        public NotesRepository(IHttpContextAccessor contextAccessor,
            ApplicationDbContext dbContext,
            PicturesLoader picturesLoader,
            RandomGenerator randomGenerator)
        {
            _contextAccessor = contextAccessor;
            _dbContext = dbContext;
            _picturesLoader = picturesLoader;
            _randomGenerator = randomGenerator;
        }

        public async Task<IEnumerable<NoteModel>> GetAllNotes()
        {
            return await _dbContext.Notes.AsNoTracking().ToListAsync();
        }

        public async Task<NoteModel?> GetNote(string noteId)
        {
            return await _dbContext.Notes.FirstOrDefaultAsync(x => x.NoteId == noteId);
        }

        public async Task<NoteModel?> GetNoteNoTracking(string noteId)
        {
            return await _dbContext.Notes.AsNoTracking().FirstOrDefaultAsync(x => x.NoteId == noteId);
        }

        public async Task<UserModel?> GetNoteAuthor(NoteModel? note)
        {
            if (note == null)
            {
                return null;
            }

            return await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == note.UserId);
        }

        public async Task<int> GetNoteScore(string noteId)
        {
            return await _dbContext.Ratings.AsNoTracking().Where(x => x.NoteId == noteId).SumAsync(x => x.Score);
        }

        public async Task<IEnumerable<string>> GetNoteTags(string noteId)
        {
            return await _dbContext.TagsForNotes.AsNoTracking().Where(x => x.NoteId == noteId).Select(x => x.Tag).ToListAsync();
        }

        public async Task<NoteImageModel> GetNoteFirstImage(string noteId)
        {
            var firstNoteImage = await _dbContext.NoteImages.AsNoTracking().OrderBy(x => x.UploadTime).FirstOrDefaultAsync(x => x.NoteId == noteId);
            if (firstNoteImage == null)
            {
                return _picturesLoader.GetDefaultNoteImage();
            }
            
            return firstNoteImage;
        }

        public async Task<IEnumerable<NoteImageModel>> GetNoteImages(string noteId)
        {
            return await _dbContext.NoteImages.Where(x => x.NoteId == noteId).OrderBy(x => x.UploadTime).ToListAsync();
        }

        public async Task<IEnumerable<NoteImageModel>> GetNoteImagesNoTracking(string noteId)
        {
            return await _dbContext.NoteImages.AsNoTracking().Where(x => x.NoteId == noteId).OrderBy(x => x.UploadTime).ToListAsync();
        }

        public async Task<NotesListViewModel> GetNotesList()
        {
            var allNotes = await GetAllNotes();

            var noteDetails = new List<NoteDetailsViewModel>();
            foreach (var note in allNotes)
            {
                var author = await GetNoteAuthor(note);
                var noteId = note.NoteId;
                var noteSummary = new NoteDetailsViewModel()
                {
                    Note = note,
                    Score = await GetNoteScore(noteId),
                    Tags = await GetNoteTags(noteId),
                    Images = new List<NoteImageModel>()
                    {
                        await GetNoteFirstImage(noteId)
                    },
                    Author = author,
                    ProfilePicture = await _picturesLoader.GetUserCurrentProfilePicture(author)
                };

                noteDetails.Add(noteSummary);
            }

            return new NotesListViewModel()
            {
                NotesDetails = noteDetails
            };
        }

        public async Task<NoteDetailsViewModel> GetNoteDetails(string noteId)
        {
            var note = await GetNoteNoTracking(noteId);
            var author = await GetNoteAuthor(note);

            return new NoteDetailsViewModel()
            {
                Note = note,
                Score = await GetNoteScore(noteId),
                Tags = await GetNoteTags(noteId),
                Images = await GetNoteImagesNoTracking(noteId),
                Author = author,
                ProfilePicture = await _picturesLoader.GetUserCurrentProfilePicture(author)
            };
        }

        public async Task<bool> Create(CreateNoteViewModel createNoteVM)
        {
            var currentUser = _contextAccessor.HttpContext?.User;
            if (currentUser == null)
            {
                return false;
            }

            var currentUserId = currentUser.GetUserId();
            if (currentUserId == string.Empty)
            {
                return false;
            }

            var note = new NoteModel()
            {
                NoteId = _randomGenerator.GetRandomId(),
                UserId = currentUserId,
                Title = createNoteVM.Title,
                Description = createNoteVM.Description,
            };
            await _dbContext.AddAsync(note);

            foreach (var image in createNoteVM.Images)
            {
                var noteImage = await _picturesLoader.LoadNoteImage(image, note);
                await _dbContext.AddAsync(noteImage);
            }

            return await Save();
        }

        public async Task<bool> Update(NoteModel note, EditNoteViewModel editNoteVM)
        {
            var currentUser = _contextAccessor.HttpContext?.User;
            if (currentUser == null)
            {
                return false;
            }

            var currentUserId = currentUser.GetUserId();
            if (currentUserId == string.Empty)
            {
                return false;
            }

            if (!currentUser.IsOwner(note) &&
                !currentUser.IsAdmin())
            {
                return false;
            }

            var newImages = editNoteVM.Images;
            if (newImages.Count != 0)
            {
                var oldImages = await GetNoteImages(note.NoteId);
                foreach (var oldImage in oldImages)
                {
                    var previousImage = new PreviousNoteImageModel()
                    {
                        Id = _randomGenerator.GetRandomId(),
                        FormerImageId = oldImage.ImageId,
                        NoteId = oldImage.NoteId,
                        ImageFileName = oldImage.ImageFileName
                    };

                    await _dbContext.PreviousNoteImages.AddAsync(previousImage);
                    _dbContext.NoteImages.Remove(oldImage);
                }

                foreach (var newImage in newImages)
                {
                    var newNoteImage = await _picturesLoader.LoadNoteImage(newImage, note);
                    await _dbContext.AddAsync(newNoteImage);
                }
            }

            var updatedNote = new NoteModel()
            {
                NoteId = note.NoteId,
                UserId = currentUserId,
                Title = editNoteVM.Title,
                Description = editNoteVM.Description
            };

            _dbContext.Notes.Update(updatedNote);

            return await Save();
        }

        public async Task<bool> Delete(DeleteNoteViewModel deleteNoteVM)
        {
            var currentUser = _contextAccessor.HttpContext?.User;
            if (currentUser == null)
            {
                return false;
            }

            var currentUserId = currentUser.GetUserId();
            if (currentUserId == string.Empty)
            {
                return false;
            }

            if (deleteNoteVM.NoteDetails == null ||
                deleteNoteVM.NoteDetails.Note == null)
            {
                return false;
            }

            var note = deleteNoteVM.NoteDetails.Note;

            if (!currentUser.IsOwner(note) &&
                !currentUser.IsAdmin())
            {
                return false;
            }

            var noteModelArchiveCopy = new PreviousNoteModel()
            {
                Id = _randomGenerator.GetRandomId(),
                FormerId = deleteNoteVM.NoteId,
                UserId = currentUserId,
                Title = note.Title,
                Description = note.Description
            };

            await _dbContext.PreviousNotes.AddAsync(noteModelArchiveCopy);
            _dbContext.Notes.Remove(note);

            return await Save();
        }

        public async Task<bool> Save()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}