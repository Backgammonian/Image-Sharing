using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.Repository.Interfaces;

namespace MyWebApp.Repository
{
    public sealed class NotesRepository : INotesRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public NotesRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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

            var images = await _dbContext.Images.Where(x => x.NoteId == noteId).ToListAsync();
            var score = await _dbContext.Ratings.Where(x => x.NoteId == noteId).SumAsync(x => x.Score);
            var tags = await _dbContext.TagsForNotes.Where(x => x.NoteId == noteId).Select(x => x.Tag).ToListAsync();
            var noteDetails = new NoteDetails(note, images, user, score, tags);

            return noteDetails;
        }
    }
}