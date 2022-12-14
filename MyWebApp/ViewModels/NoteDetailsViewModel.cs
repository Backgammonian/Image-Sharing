using MyWebApp.Models;

namespace MyWebApp.ViewModels
{
    public sealed class NoteDetailsViewModel
    {
        public NoteModel? Note { get; set; }
        public int Score { get; set; }
        public ThreadOfNoteModel? Thread { get; set; }
        public IEnumerable<NoteImageModel> Images { get; set; } = Enumerable.Empty<NoteImageModel>();
        public UserModel? Author { get; set; }
        public UserImageModel ProfilePicture { get; set; } = new UserImageModel();
    }
}
