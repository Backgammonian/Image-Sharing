using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.TableModels
{
    public sealed class UserModel : IdentityUser
    {
        public string Status { get; set; } = string.Empty;
    }
}
