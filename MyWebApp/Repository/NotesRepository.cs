using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Extensions;
using MyWebApp.Models;
using MyWebApp.PicturesModule;
using MyWebApp.ViewModels;

namespace MyWebApp.Repository
{
    public sealed class NotesRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly PicturesLoader _picturesLoader;
        private readonly RandomGenerator _randomGenerator;
        private readonly CredentialsRepository _credentialsRepository;

        public NotesRepository(ApplicationDbContext dbContext,
            PicturesLoader picturesLoader,
            RandomGenerator randomGenerator,
            CredentialsRepository credentialsRepository)
        {
            _dbContext = dbContext;
            _picturesLoader = picturesLoader;
            _randomGenerator = randomGenerator;
            _credentialsRepository = credentialsRepository;
        }

        public async Task<IEnumerable<ThreadModel>> GetAvailableNoteThreads()
        {
            return await _dbContext.Threads.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<NoteModel>> GetNotesSlice(int offset, int size)
        {
            return await _dbContext.Notes.AsNoTracking().Skip(offset).Take(size).ToListAsync();
        }

        public async Task<int> GetCount()
        {
            return await _dbContext.Notes.CountAsync();
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

        public async Task<NoteThreadModel?> GetNoteThread(string noteId)
        {
            return await _dbContext.NoteThreads.AsNoTracking().Where(x => x.NoteId == noteId).FirstOrDefaultAsync();
        }

        public async Task<NoteImageModel?> GetNoteFirstImage(string noteId)
        {
            return await _dbContext.NoteImages.AsNoTracking().OrderBy(x => x.UploadTime).FirstOrDefaultAsync(x => x.NoteId == noteId);
        }

        public async Task<IEnumerable<NoteImageModel>> GetNoteImages(string noteId)
        {
            return await _dbContext.NoteImages.AsNoTracking().Where(x => x.NoteId == noteId).OrderBy(x => x.UploadTime).ToListAsync();
        }

        public async Task<IEnumerable<NoteImageModel>> GetNoteImagesNoTracking(string noteId)
        {
            return await _dbContext.NoteImages.AsNoTracking().Where(x => x.NoteId == noteId).OrderBy(x => x.UploadTime).ToListAsync();
        }

        public async Task<IEnumerable<NoteSummaryViewModel>> GetNotesSummaries(int offset, int size)
        {
            var notes = await GetNotesSlice(offset, size);

            var noteSummaries = new List<NoteSummaryViewModel>();
            foreach (var note in notes)
            {
                var author = await GetNoteAuthor(note);
                var noteId = note.NoteId;
                var noteSummary = new NoteSummaryViewModel()
                {
                    Note = note,
                    Thread = await GetNoteThread(noteId),
                    FirstImage = await GetNoteFirstImage(noteId),
                    Author = author,
                    ProfilePicture = await _picturesLoader.GetUserCurrentProfilePicture(author)
                };

                noteSummaries.Add(noteSummary);
            }

            return noteSummaries;
        }

        public async Task<NoteDetailsViewModel> GetNoteDetails(string noteId)
        {
            var note = await GetNoteNoTracking(noteId);
            var author = await GetNoteAuthor(note);

            return new NoteDetailsViewModel()
            {
                Note = note,
                Thread = await GetNoteThread(noteId),
                Images = await GetNoteImagesNoTracking(noteId),
                Author = author,
                ProfilePicture = await _picturesLoader.GetUserCurrentProfilePicture(author),
            };
        }

        public async Task<bool> Create(CreateNoteViewModel createNoteVM)
        {
            var credentials = await _credentialsRepository.GetLoggedInUser();
            var currentUser = credentials.User;
            var claims = credentials.ClaimsPrincipal;

            if (currentUser == null ||
                claims == null)
            {
                return false;
            }

            var note = new NoteModel()
            {
                NoteId = _randomGenerator.GetRandomId(),
                UserId = currentUser.Id,
                Title = createNoteVM.Title,
                Description = createNoteVM.Description,
            };

            await _dbContext.Notes.AddAsync(note);

            foreach (var image in createNoteVM.Images)
            {
                var noteImage = await _picturesLoader.LoadNoteImage(image, note);
                await _dbContext.NoteImages.AddAsync(noteImage);
            }

            var selectedThread = createNoteVM.SelectedThread;
            if (selectedThread != string.Empty)
            {
                var availableThreads = await GetAvailableNoteThreads();
                if (availableThreads.Any(x => x.Thread == selectedThread))
                {
                    await _dbContext.NoteThreads.AddAsync(new NoteThreadModel()
                    {
                        Id = _randomGenerator.GetRandomId(),
                        Thread = selectedThread,
                        NoteId = note.NoteId
                    });
                }
            }

            return await Save();
        }

        public async Task<bool> Update(NoteModel note, EditNoteViewModel editNoteVM)
        {
            var credentials = await _credentialsRepository.GetLoggedInUser();
            var currentUser = credentials.User;
            var claims = credentials.ClaimsPrincipal;

            if (currentUser == null ||
                claims == null ||
                !claims.IsOwner(note) &&
                !claims.IsAdmin())
            {
                return false;
            }

            var newImages = editNoteVM.Images;
            if (newImages.Any())
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
                    await _dbContext.NoteImages.AddAsync(newNoteImage);
                }
            }

            var selectedThread = editNoteVM.SelectedThread;
            if (selectedThread != string.Empty)
            {
                var availableThreads = await GetAvailableNoteThreads();
                if (availableThreads.Any(x => x.Thread == selectedThread))
                {
                    await _dbContext.NoteThreads.AddAsync(new NoteThreadModel()
                    {
                        Id = _randomGenerator.GetRandomId(),
                        Thread = selectedThread,
                        NoteId = note.NoteId
                    });

                    var oldThreadOfNote = await GetNoteThread(note.NoteId);
                    if (oldThreadOfNote != null)
                    {
                        _dbContext.NoteThreads.Remove(oldThreadOfNote);
                    }
                }
            }

            var updatedNote = new NoteModel()
            {
                NoteId = note.NoteId,
                UserId = currentUser.Id,
                Title = editNoteVM.Title,
                Description = editNoteVM.Description
            };

            _dbContext.Notes.Update(updatedNote);

            return await Save();
        }

        public async Task<bool> Delete(DeleteNoteViewModel deleteNoteVM)
        {
            if (deleteNoteVM.NoteDetails == null ||
                deleteNoteVM.NoteDetails.Note == null) 
            {
                return false;
            }

            var note = deleteNoteVM.NoteDetails.Note;

            var credentials = await _credentialsRepository.GetLoggedInUser(true);
            var currentUser = credentials.User;
            var claims = credentials.ClaimsPrincipal;

            if (currentUser == null ||
                claims == null ||
                !claims.IsOwner(note) &&
                !claims.IsAdmin())
            {
                return false;
            }

            var noteModelArchiveCopy = new PreviousNoteModel()
            {
                Id = _randomGenerator.GetRandomId(),
                FormerId = deleteNoteVM.NoteId,
                UserId = currentUser.Id,
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