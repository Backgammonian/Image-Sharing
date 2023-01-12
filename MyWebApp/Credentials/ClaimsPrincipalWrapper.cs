using MyWebApp.Models;
using System.Security.Claims;

namespace MyWebApp.Credentials
{
    public sealed class ClaimsPrincipalWrapper
    {
        private readonly ClaimsPrincipal? _user;

        public ClaimsPrincipalWrapper(ClaimsPrincipal? user)
        {
            _user = user;
        }

        public bool IsAuthenticated()
        {
            return _user != null &&
                _user.Identity != null &&
                _user.Identity.IsAuthenticated;
        }

        public bool IsNotAuthenticated()
        {
            return !IsAuthenticated();
        }

        public string GetUserId()
        {
            if (_user == null ||
                IsNotAuthenticated())
            {
                return string.Empty;
            }

            var claim = _user.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                return string.Empty;
            }

            return claim.Value;
        }

        public bool IsAdmin()
        {
            return _user != null &&
                IsAuthenticated() &&
                _user.IsInRole(UserRoles.Admin);
        }

        public bool IsOwner(NoteModel note)
        {
            return _user != null &&
                IsAuthenticated() &&
                (_user.IsInRole(UserRoles.User) || _user.IsInRole(UserRoles.Admin)) &&
                note.UserId == GetUserId();
        }
    }
}
