using MyWebApp.Models;
using MyWebApp.ViewModels;

namespace MyWebApp.Repository.Interfaces
{
    public interface IThreadsRepository
    {
        Task<IEnumerable<NoteThreadModel>> GetNotesFromThread(string thread, int offset, int size);
        Task<int> GetCountOfNotesFromThread(string thread);
        Task<IEnumerable<ThreadModel>> GetAllThreads();
        Task<NotesFromThreadViewModel> GetByThread(string thread, int offset, int size);
        Task<bool> Create(CreateThreadViewModel createThreadVM);
        Task<bool> Delete(DeleteThreadViewModel deleteThreadVM);
        Task<bool> Save();
    }
}
