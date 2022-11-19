using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.TableModels;

namespace MyWebApp.Repository
{
    public sealed class TagsRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TagsRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<NotesMarkedByTag?> GetByTag(string? tag)
        {
            if (tag == null ||
                tag == string.Empty)
            {
                return null;
            }

            var idsOfNotesWithTag = await _dbContext.TagsForNotes.Where(x => x.Tag == tag).ToListAsync();
            var notes = new List<NoteModel>();
            for (int i = 0; i < idsOfNotesWithTag.Count; i++)
            {
                var note = await _dbContext.Notes.FirstOrDefaultAsync(x => x.NoteId == idsOfNotesWithTag[i].NoteId);
                if (note != null)
                {
                    notes.Add(note);
                }
            }

            var tagsAndScoresOfNotes = new List<TagsAndScoreOfNote>();
            foreach (var note in notes)
            {
                var noteScore = await _dbContext.Ratings.Where(x => x.NoteId == note.NoteId).SumAsync(x => x.Score);
                var noteTags = await _dbContext.TagsForNotes.Where(x => x.NoteId == note.NoteId).Select(x => x.Tag).ToListAsync();
                tagsAndScoresOfNotes.Add(new TagsAndScoreOfNote(note, noteScore, noteTags));
            }

            return new NotesMarkedByTag(tag, tagsAndScoresOfNotes);
        }
    }
}
