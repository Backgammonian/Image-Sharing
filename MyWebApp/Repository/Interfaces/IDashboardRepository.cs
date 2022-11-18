using MyWebApp.Models;

namespace MyWebApp.Repository.Interfaces
{
    public interface IDashboardRepository
    {
        Task<IEnumerable<NoteDetails>> GetAllUserNotes();
        Task<IEnumerable<UserRatings>> GetAllUserRatings();
    }
}
