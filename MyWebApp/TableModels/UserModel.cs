using Microsoft.AspNetCore.Identity;

namespace MyWebApp.TableModels
{
    public sealed class UserModel : IdentityUser
    {
        public string Status { get; set; } = string.Empty;
    }
}
