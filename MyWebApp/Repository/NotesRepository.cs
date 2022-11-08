using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.Repository.Interfaces;
using MyWebApp.TableModels;

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

        public async Task<NoteDetails?> GetNoteDetails(string? noteId)
        {
            if (noteId == null ||
                noteId == string.Empty)
            {
                return null;
            }

            var note = await _dbContext.Notes.FirstOrDefaultAsync(x => x.NoteId == noteId);
            if (note == null)
            {
                return null;
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == note.UserId);
            if (user == null)
            {
                return null;
            }

            var profileImage = await _dbContext.ProfileImages.OrderBy(x => x.UploadTime).LastOrDefaultAsync(x => x.UserId == user.UserId);
            profileImage ??= _picturesLoader.GetDefaultProfileImage();

            var images = await _dbContext.NoteImages.Where(x => x.NoteId == noteId).ToListAsync();
            var score = await _dbContext.Ratings.Where(x => x.NoteId == noteId).SumAsync(x => x.Score);
            var tags = await _dbContext.TagsForNotes.Where(x => x.NoteId == noteId).Select(x => x.Tag).ToListAsync();
            var noteDetails = new NoteDetails(note, images, user, profileImage, score, tags);

            return noteDetails;
        }

        public async Task<bool> Create(CreateNoteViewModel createNoteVM)
        {
            //TODO
            var user = await _dbContext.Users.FirstOrDefaultAsync();
            var noteModel = new NoteModel()
            {
                NoteId = createNoteVM.NoteId,
                UserId = user.UserId, //PLACEHOLDER
                Title = createNoteVM.Title,
                Description = createNoteVM.Description,
            };
            await _dbContext.AddAsync(noteModel);

            foreach (var image in createNoteVM.Images)
            {
                await _dbContext.AddAsync(_picturesLoader.LoadNoteImage(image, noteModel));
            }

            return await Save();
        }

        public async Task<bool> Update(NoteModel note)
        {
            //TODO
            return await Save();
        }

        public async Task<bool> Delete(NoteModel note)
        {
            //TODO
            return await Save();
        }

        public async Task<bool> Save()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}