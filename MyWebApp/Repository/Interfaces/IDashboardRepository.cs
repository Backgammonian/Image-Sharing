using MyWebApp.Models;
using MyWebApp.ViewModels;

namespace MyWebApp.Repository.Interfaces
{
    public interface IDashboardRepository
    {
        Task<int> GetNotesCount(UserModel? user);
        Task<DashboardViewModel> GetDashboard(UserModel user, int offset, int size);
        Task<bool> Update(UserModel user, EditUserProfileViewModel editUserProfileVM);
        Task<bool> UpdatePassword(string userId, EditPasswordViewModel editPasswordVM);
        Task<bool> Save();
    }
}
