using MyWebApp.TableModels;

namespace MyWebApp.Models
{
    public sealed class UserDetails
    {
        public UserDetails(UserModel user, UserImageModel profilePicture)
        {
            User = user;
            ProfilePicture = profilePicture;
        }

        public UserModel User { get; }
        public UserImageModel ProfilePicture { get; }
    }
}
