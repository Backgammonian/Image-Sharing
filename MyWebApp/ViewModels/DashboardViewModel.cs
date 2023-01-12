using MyWebApp.Models;

namespace MyWebApp.ViewModels
{
    public class DashboardViewModel
    {
        public IEnumerable<NoteDetailsViewModel> UserNotes { get; set; } = Enumerable.Empty<NoteDetailsViewModel>();
        public UserModel User { get; set; } = new UserModel();
        public UserImageModel ProfilePicture { get; set; } = new UserImageModel();
        public PagingViewModel PagingViewModel { get; set; } = new PagingViewModel();
    }
}