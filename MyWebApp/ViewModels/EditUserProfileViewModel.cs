using MyWebApp.Models;

namespace MyWebApp.ViewModels
{
    public sealed class EditUserProfileViewModel
    {
        public UserImageModel ProfilePicture { get; set; } = new UserImageModel();
        public string Status { get; set; } = string.Empty;
        public IFormFile? NewProfilePicture { get; set; }
    }
}
