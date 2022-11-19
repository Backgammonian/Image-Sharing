using MyWebApp.TableModels;

namespace MyWebApp.ViewModels
{
    public sealed class UserRatingsViewModel
    {
        public UserModel? User { get; set; }
        public UserImageModel? ProfilePicture { get; set; }
        public IEnumerable<NoteRatingViewModel> UsersRatingsOfNotes { get; set; } = Enumerable.Empty<NoteRatingViewModel>();
    }
}
