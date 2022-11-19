using MyWebApp.TableModels;

namespace MyWebApp.ViewModels
{
    public sealed class NoteDetailsViewModel
    {
        public NoteModel? Note { get; set; }
        public int Score { get; set; }
        public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<NoteImageModel> Images { get; set; } = Enumerable.Empty<NoteImageModel>();
        public UserModel? Author { get; set; }
        public UserImageModel? ProfilePicture { get; set; }
    }
}
