using MyWebApp.Credentials;
using MyWebApp.Models;

namespace MyWebApp.ViewModels
{
    public class CredentialsViewModel
    {
        public UserModel? User { get; set; }
        public ClaimsPrincipalWrapper? Credentials { get; set; }
    }
}