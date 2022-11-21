using MyWebApp.Data;
using MyWebApp.Models;
using System.Security.Claims;

namespace MyWebApp.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal? user)
        {
            if (user == null ||
                !user.IsAuthenticated())
            {
                return string.Empty;
            }

            var claim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                return string.Empty;
            }

            return claim.Value;
        }

        public static bool IsAuthenticated(this ClaimsPrincipal? user)
        {
            return user != null &&
                user.Identity != null &&
                user.Identity.IsAuthenticated;
        }

        public static bool IsAdmin(this ClaimsPrincipal? user)
        {
            return user != null &&
                user.IsAuthenticated() &&
                user.IsInRole(UserRoles.Admin);
        }

        public static bool IsOwner(this ClaimsPrincipal? user, NoteModel note)
        {
            return user != null &&
                user.IsAuthenticated() &&
                user.IsInRole(UserRoles.User) &&
                note.UserId == user.GetUserId();
        }
    }
}
