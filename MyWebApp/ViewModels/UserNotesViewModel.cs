using MyWebApp.Models;

namespace MyWebApp.ViewModels
{
    public class UserNotesViewModel
    {
        public UserModel? User { get; set; }
        public UserImageModel ProfilePicture { get; set; } = new UserImageModel();
        public IEnumerable<NoteDetailsViewModel> Notes { get; set; } = Enumerable.Empty<NoteDetailsViewModel>();
        public PagingViewModel PagingViewModel { get; set; } = new PagingViewModel();
    }
}
