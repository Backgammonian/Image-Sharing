using MyWebApp.Models;

namespace MyWebApp.ViewModels
{
    public sealed class UserSummaryViewModel
    {
        public UserModel? User { get; set; }
        public UserImageModel ProfilePicture { get; set; } = new UserImageModel();
    }
}
