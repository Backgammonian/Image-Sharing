using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Repository.Interfaces;
using MyWebApp.ViewModels;
using MyWebApp.Credentials;

namespace MyWebApp.Repository
{
    public sealed class CredentialsRepository : ICredentialsRepository
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ApplicationDbContext _dbContext;

        public CredentialsRepository(IHttpContextAccessor contextAccessor,
            ApplicationDbContext dbContext)
        {
            _contextAccessor = contextAccessor;
            _dbContext = dbContext;
        }

        public async Task<CredentialsViewModel> GetLoggedInUser(bool asNoTracking = true)
        {
            var credentialsVM = new CredentialsViewModel();

            var claimsPrincipal = _contextAccessor.HttpContext?.User;
            if (claimsPrincipal == null)
            {
                return credentialsVM;
            }

            var credentials = new ClaimsPrincipalWrapper(claimsPrincipal);
            if (credentials.IsNotAuthenticated())
            {
                return credentialsVM;
            }

            var currentUserId = credentials.GetUserId();
            if (currentUserId == string.Empty)
            {
                return credentialsVM;
            }

            return new CredentialsViewModel() 
            { 
                User = asNoTracking ?
                    await _dbContext.Users
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == currentUserId) :
                    await _dbContext.Users
                        .FirstOrDefaultAsync(x => x.Id == currentUserId),
                Credentials = credentials
            };
        }
    }
}
