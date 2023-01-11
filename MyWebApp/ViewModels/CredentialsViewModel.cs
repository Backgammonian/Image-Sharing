using MyWebApp.Models;
using System.Security.Claims;

namespace MyWebApp.ViewModels
{
    public sealed class CredentialsViewModel
    {
        public UserModel? User { get; set; }
        public ClaimsPrincipal? ClaimsPrincipal { get; set; }
    }
}
