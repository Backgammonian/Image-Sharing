using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.Repository.Interfaces;
using MyWebApp.ViewModels;

namespace MyWebApp.Repository
{
    public sealed class ThreadsRepository : IThreadsRepository
    {
        private readonly INotesRepository _notesRepository;
        private readonly ApplicationDbContext _dbContext;

        public ThreadsRepository(INotesRepository notesRepository,
            ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _notesRepository = notesRepository;
        }

        public async Task<IEnumerable<NoteThreadModel>> GetNotesFromThread(string thread, int offset, int size)
        {
            return await _dbContext.NoteThreads
                .AsNoTracking()
                .Where(x => x.ThreadId == thread)
                .Skip(offset)
                .Take(size).ToListAsync();
        }

        public async Task<int> GetCountOfNotesFromThread(string thread)
        {
            return await _dbContext.NoteThreads
                .CountAsync(x => x.ThreadId == thread);
        }

        public async Task<IEnumerable<ThreadModel>> GetAllThreads()
        {
            return await _dbContext.Threads
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<NotesFromThreadViewModel> GetByThread(string thread, int offset, int size)
        {
            var notesFromThread = await GetNotesFromThread(thread, offset, size);

            var threadNotesDetailsList = new List<NoteDetailsViewModel>();
            foreach (var noteFromThread in notesFromThread)
            {
                var note = await _notesRepository.GetNoteNoTracking(noteFromThread.NoteId);
                if (note != null)
                {
                    var noteDetails = await _notesRepository.GetNoteDetails(note.NoteId);
                    if (noteDetails != null)
                    {
                        threadNotesDetailsList.Add(noteDetails);
                    }
                }
            }

            return new NotesFromThreadViewModel()
            {
                Thread = thread,
                NotesDetails = threadNotesDetailsList
            };
        }

        public async Task<bool> Create(CreateThreadViewModel createThreadVM)
        {
            var newThreadName = createThreadVM.NewThreadName.ToLower();
            if (await _dbContext.Threads.AnyAsync(x => x.Thread == newThreadName))
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
            var threadName = deleteThreadVM.SelectedThreadName.ToLower();
            var thread = await _dbContext.Threads
                .FirstOrDefaultAsync(x => x.Thread == threadName);

            if (thread == null)
            {
                return false;
            }

            _dbContext.Threads.Remove(thread);

            var noteThreads = await _dbContext.NoteThreads
                .Where(x => x.ThreadId == threadName)
                .ToListAsync();

            foreach (var noteThread in noteThreads)
            {
                _dbContext.NoteThreads.Remove(noteThread);
            }

            return await Save();
        }

        public async Task<bool> Save()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
