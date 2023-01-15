using Microsoft.AspNetCore.Identity;

namespace MyWebApp.Models
{
    public class UserModel : IdentityUser
    {
        public string Status { get; set; } = string.Empty;
    }
}
