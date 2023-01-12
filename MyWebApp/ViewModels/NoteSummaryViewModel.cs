using MyWebApp.Models;

namespace MyWebApp.ViewModels
{
    public class NoteSummaryViewModel
    {
        public NoteModel? Note { get; set; }
        public NoteThreadModel? Thread { get; set; }
        public NoteImageModel? FirstImage { get; set; }
        public UserModel? Author { get; set; }
        public UserImageModel ProfilePicture { get; set; } = new UserImageModel();
    }
}
