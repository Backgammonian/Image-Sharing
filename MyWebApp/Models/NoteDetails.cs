using MyWebApp.TableModels;

namespace MyWebApp.Models
{
    public sealed class NoteDetails
    {
        public NoteDetails(NoteModel noteModel,
            IEnumerable<NoteImageModel> images,
            UserModel user,
            UserImageModel profileImage,
            int score,
            IEnumerable<string> tags)
        {
            Title = noteModel.Title;
            Description = noteModel.Description;
            AuthorId = user.UserId;
            AuthorName = user.Name;
            ProfilePicture = profileImage;
            Images = images;
            Score = score;
            Tags = tags;
        }

        public string Title { get; }
        public string Description { get; }
        public string AuthorId { get; }
        public string AuthorName { get; }
        public UserImageModel ProfilePicture { get; }
        public IEnumerable<NoteImageModel> Images { get; }
        public int Score { get; }
        public IEnumerable<string> Tags { get; }
    }
}
