using MyWebApp.Models;
using MyWebApp.TableModels;

namespace MyWebApp.Repository.Interfaces
{
    public interface IUsersRepository
    {
        Task<UserDetails?> GetUserInfo(string userId);
        Task<UserNotes?> GetUserNotes(string userId);
        Task<UserRatings?> GetUserRatings(string userId);
    }
}
