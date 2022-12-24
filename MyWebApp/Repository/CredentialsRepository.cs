using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Extensions;
using MyWebApp.ViewModels;

namespace MyWebApp.Repository
{
    public sealed class CredentialsRepository
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ApplicationDbContext _dbContext;

        public CredentialsRepository(IHttpContextAccessor contextAccessor, ApplicationDbContext dbContext)
        {
            _contextAccessor = contextAccessor;
            _dbContext = dbContext;
        }

        public async Task<CredentialsViewModel> GetLoggedInUser(bool asNoTracking = true)
        {
            var credentials = new CredentialsViewModel();

            var currentUser = _contextAccessor.HttpContext?.User;
            if (currentUser == null)
            {
                return credentials;
            }

            if (!currentUser.IsAuthenticated())
            {
                return credentials;
            }

            var currentUserId = currentUser.GetUserId();
            if (currentUserId == string.Empty)
            {
                return credentials;
            }

            return new CredentialsViewModel() 
            { 
                User = asNoTracking ?
                    await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentUserId) :
                    await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == currentUserId),
                ClaimsPrincipal = currentUser
            };
        }
    }
}
