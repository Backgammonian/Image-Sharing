using MyWebApp.Models;

namespace MyWebApp.ViewModels
{
    public sealed class UserRatingsViewModel
    {
        public UserModel? User { get; set; }
        public UserImageModel ProfilePicture { get; set; } = new UserImageModel();
        public IEnumerable<NoteRatingViewModel> UserRatingsOfNotes { get; set; } = Enumerable.Empty<NoteRatingViewModel>();
    }
}
