using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.ViewModels;

namespace MyWebApp.Repository
{
    public sealed class TagsRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly NotesRepository _notesRepository;

        public TagsRepository(ApplicationDbContext dbContext,
            NotesRepository notesRepository)
        {
            _dbContext = dbContext;
            _notesRepository = notesRepository;
        }

        public async Task<IEnumerable<TagsForNotesModel>> GetTaggedNotes(string tag)
        {
            return await _dbContext.TagsForNotes.Where(x => x.Tag == tag).ToListAsync();
        }

        public async Task<TaggedNotesViewModel> GetByTag(string tag)
        {
            var taggedNotes = await GetTaggedNotes(tag);

            var notes = new List<NoteModel>();
            foreach (var taggedNote in taggedNotes)
            {
                var note = await _notesRepository.GetNoteNoTracking(taggedNote.NoteId);
                if (note != null)
                {
                    notes.Add(note);
                }
            }

            var taggedNotesDetails = new List<NoteDetailsViewModel>();
            foreach (var taggedNote in taggedNotes)
            {
                var note = await _notesRepository.GetNoteNoTracking(taggedNote.NoteId);
                if (note != null)
                {
                    var noteDetails = await _notesRepository.GetNoteDetails(note.NoteId);
                    if (noteDetails != null)
                    {
                        taggedNotesDetails.Add(noteDetails);
                    }
                }
            }

            return new TaggedNotesViewModel()
            {
                Tag = tag,
                TaggedNotesDetails = taggedNotesDetails
            };
        }
    }
}
