using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.ViewModels;

namespace MyWebApp.Repository
{
    public sealed class ThreadsRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly NotesRepository _notesRepository;

        public ThreadsRepository(ApplicationDbContext dbContext,
            NotesRepository notesRepository)
        {
            _dbContext = dbContext;
            _notesRepository = notesRepository;
        }

        public async Task<IEnumerable<ThreadOfNoteModel>> GetNotesFromThread(string thread)
        {
            return await _dbContext.ThreadsOfNotes.Where(x => x.Thread == thread).ToListAsync();
        }

        public async Task<NotesFromThreadViewModel> GetByThread(string thread)
        {
            var threadNotes = await GetNotesFromThread(thread);

            var notes = new List<NoteModel>();
            foreach (var threadNote in threadNotes)
            {
                var note = await _notesRepository.GetNoteNoTracking(threadNote.NoteId);
                if (note != null)
                {
                    notes.Add(note);
                }
            }

            var threadNotesDetails = new List<NoteDetailsViewModel>();
            foreach (var threadNote in threadNotes)
            {
                var note = await _notesRepository.GetNoteNoTracking(threadNote.NoteId);
                if (note != null)
                {
                    var noteDetails = await _notesRepository.GetNoteDetails(note.NoteId);
                    if (noteDetails != null)
                    {
                        threadNotesDetails.Add(noteDetails);
                    }
                }
            }

            return new NotesFromThreadViewModel()
            {
                Thread = thread,
                NotesDetails = threadNotesDetails
            };
        }
    }
}
