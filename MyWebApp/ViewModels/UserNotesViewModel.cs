using MyWebApp.Models;

namespace MyWebApp.ViewModels
{
    public sealed class UserNotesViewModel
    {
        public UserModel? User { get; set; }
        public UserImageModel? ProfilePicture { get; set; }
        public IEnumerable<NoteDetailsViewModel> Notes { get; set; } = Enumerable.Empty<NoteDetailsViewModel>();
    }
}
