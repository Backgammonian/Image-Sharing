using MyWebApp.ViewModels;

namespace MyWebApp.Repository.Interfaces
{
    public interface ICredentialsRepository
    {
        Task<CredentialsViewModel> GetLoggedInUser(bool asNoTracking = true);
    }
}
