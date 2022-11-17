using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.Repository.Interfaces;
using MyWebApp.TableModels;
using MyWebApp.ViewModels;

namespace MyWebApp.Repository
{
    public sealed class NotesRepository : INotesRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly PicturesLoader _picturesLoader;

        public NotesRepository(ApplicationDbContext dbContext, PicturesLoader picturesLoader)
        {
            _dbContext = dbContext;
            _picturesLoader = picturesLoader;
        }

        public async Task<IEnumerable<NoteSummary>> GetNotesList()
        {
            var list = new List<NoteSummary>();
            var notes = await _dbContext.Notes.ToListAsync();
            foreach (var note in notes)
            {
                var noteScore = await _dbContext.Ratings.Where(x => x.NoteId == note.NoteId).SumAsync(x => x.Score);
                var noteTags = await _dbContext.TagsForNotes.Where(x => x.NoteId == note.NoteId).Select(x => x.Tag).ToListAsync();
                list.Add(new NoteSummary(note, noteScore, noteTags));
            }

            return list;
        }

        public async Task<NoteDetails?> GetNoteDetails(string noteId)
        {
            var note = await _dbContext.Notes.FirstOrDefaultAsync(x => x.NoteId == noteId);
            if (note == null)
            {
                return null;
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == note.UserId);
            if (user == null)
            {
                return null;
            }

            var profileImage = await _dbContext.ProfileImages.OrderBy(x => x.UploadTime).LastOrDefaultAsync(x => x.UserId == user.Id);
            profileImage ??= _picturesLoader.GetDefaultProfileImage();

            var images = await _dbContext.NoteImages.Where(x => x.NoteId == noteId).ToListAsync();
            var score = await _dbContext.Ratings.Where(x => x.NoteId == noteId).SumAsync(x => x.Score);
            var tags = await _dbContext.TagsForNotes.Where(x => x.NoteId == noteId).Select(x => x.Tag).ToListAsync();
            var noteDetails = new NoteDetails(note, images, user, profileImage, score, tags);

            return noteDetails;
        }

        public async Task<NoteModel?> GetNoteModel(string noteId)
        {
            return await _dbContext.Notes.FirstOrDefaultAsync(x => x.NoteId == noteId);
        }

        public async Task<NoteModel?> GetNoteModelNoTracking(string noteId)
        {
            return await _dbContext.Notes.AsNoTracking().FirstOrDefaultAsync(x => x.NoteId == noteId);
        }

        public async Task<bool> Create(CreateNoteViewModel createNoteVM)
        {
            //TODO
            var user = await _dbContext.Users.FirstOrDefaultAsync();
            var noteModel = new NoteModel()
            {
                NoteId = createNoteVM.NoteId,
                UserId = user.Id, //PLACEHOLDER
                Title = createNoteVM.Title,
                Description = createNoteVM.Description,
            };
            await _dbContext.AddAsync(noteModel);

            foreach (var image in createNoteVM.Images)
            {
                var noteImageModel = await _picturesLoader.LoadNoteImage(image, noteModel);
                await _dbContext.AddAsync(noteImageModel);
            }

            return await Save();
        }

        public async Task<bool> Update(NoteModel note, EditNoteViewModel editNoteVM)
        {
            var newImages = editNoteVM.Images;
            if (newImages.Count != 0)
            {
                var oldImages = await _dbContext.NoteImages.Where(x => x.NoteId == note.NoteId).ToListAsync();
                foreach (var oldImage in oldImages)
                {
                    var previousImage = new PreviousNoteImageModel()
                    {
                        Id = RandomGenerator.GetRandomId(),
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
                UserId = note.UserId,
                Title = editNoteVM.Title,
                Description = editNoteVM.Description
            };
            _dbContext.Notes.Update(updatedNote);

            return await Save();
        }

        public async Task<bool> Delete(NoteModel note)
        {
            var noteModelArchiveCopy = new PreviousNoteModel()
            {
                Id = RandomGenerator.GetRandomId(),
                FormerId = note.NoteId,
                UserId = note.UserId,
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