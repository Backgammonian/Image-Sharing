using MyWebApp.Models;

namespace MyWebApp.ViewModels
{
    public class UserSummaryViewModel
    {
        public UserModel? User { get; set; }
        public UserImageModel ProfilePicture { get; set; } = new UserImageModel();
    }
}
