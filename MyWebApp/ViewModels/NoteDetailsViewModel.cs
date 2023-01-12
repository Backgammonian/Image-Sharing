using MyWebApp.Models;

namespace MyWebApp.ViewModels
{
    public class NoteDetailsViewModel
    {
        public NoteModel? Note { get; set; }
        public NoteThreadModel? Thread { get; set; }
        public IEnumerable<NoteImageModel> Images { get; set; } = Enumerable.Empty<NoteImageModel>();
        public UserModel? Author { get; set; }
        public UserImageModel ProfilePicture { get; set; } = new UserImageModel();
    }
}
