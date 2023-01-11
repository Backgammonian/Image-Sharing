using MyWebApp.Models;
using MyWebApp.ViewModels;

namespace MyWebApp.Repository.Interfaces
{
    public interface IDashboardRepository
    {
        Task<UserImageModel> GetCurrentUserProfilePicture();
        Task<int> GetNotesCount();
        Task<DashboardViewModel?> GetDashboard(int offset, int size);
        Task<bool> Update(UserModel user, EditUserProfileViewModel editUserProfileVM);
        Task<bool> UpdatePassword(string userId, EditPasswordViewModel editPasswordVM);
        Task<bool> Save();
    }
}
