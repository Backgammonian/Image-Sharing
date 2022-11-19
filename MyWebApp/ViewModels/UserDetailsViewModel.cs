using MyWebApp.TableModels;

namespace MyWebApp.ViewModels
{
    public sealed class UserDetailsViewModel
    {
        public UserModel? User { get; set; }
        public UserImageModel? ProfilePicture { get; set; }
    }
}
