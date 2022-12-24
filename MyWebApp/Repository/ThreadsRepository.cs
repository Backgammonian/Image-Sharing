using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Extensions;
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

        public async Task<IEnumerable<NoteThreadModel>> GetNotesFromThread(string thread)
        {
            return await _dbContext.NoteThreads.Where(x => x.Thread == thread).ToListAsync();
        }

        public async Task<IEnumerable<ThreadModel>> GetAllThreads()
        {
            return await _dbContext.Threads.ToListAsync();
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

        public async Task<bool> Create(CreateThreadViewModel createThreadVM)
        {
            var newThreadName = createThreadVM.NewThreadName.ToLower();
            var allThreads = await GetAllThreads();
            if (allThreads.Any(x => x.Thread == newThreadName)) 
            {
                return false;
            }

            await _dbContext.Threads.AddAsync(new ThreadModel()
            {
                Thread = newThreadName
            });

            return await Save();
        }

        public async Task<bool> Delete(DeleteThreadViewModel deleteThreadVM)
        {
            var threadName = deleteThreadVM.ThreadName.ToLower();
            var allThreads = await GetAllThreads();
            if (!allThreads.Any(x => x.Thread == threadName))
            {
                return false;
            }

            _dbContext.Threads.Remove(new ThreadModel()
            {
                Thread = threadName
            });

            return await Save();
        }

        public async Task<bool> Save()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
