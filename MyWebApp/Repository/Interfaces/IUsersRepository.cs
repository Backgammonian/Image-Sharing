using MyWebApp.Models;
using MyWebApp.ViewModels;

namespace MyWebApp.Repository.Interfaces
{
    public interface IUsersRepository
    {
        Task<IEnumerable<UserModel>> GetUsersSlice(int offset, int size);
        Task<IEnumerable<UserSummaryViewModel>> GetUsers(int offset, int size);
        Task<int> GetCount();
        Task<UserModel?> GetUser(string userId);
        Task<UserModel?> GetUserNoTracking(string userId);
        Task<UserImageModel> GetUsersCurrentProfilePicture(string userId);
        Task<UserImageModel> GetUsersCurrentProfilePicture(UserModel? user);
        Task<IEnumerable<NoteModel>> GetNotesOfUser(string userId, int offset, int size);
        Task<int> GetCountOfUserNotes(string userId);
        Task<UserDetailsViewModel> GetUserDetails(string userId);
        Task<UserNotesViewModel?> GetUserNotes(string userId, int offset, int size);
    }
}
