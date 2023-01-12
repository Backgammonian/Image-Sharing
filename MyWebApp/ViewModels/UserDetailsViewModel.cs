using MyWebApp.Models;

namespace MyWebApp.ViewModels
{
    public class UserDetailsViewModel
    {
        public UserModel? User { get; set; }
        public UserImageModel ProfilePicture { get; set; } = new UserImageModel();
    }
}
