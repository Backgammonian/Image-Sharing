using MyWebApp.TableModels;

namespace MyWebApp.Models
{
    public sealed class NoteDetails
    {
        public NoteDetails(NoteModel noteModel,
            IEnumerable<ImageModel> images,
            UserModel user,
            int score,
            IEnumerable<string> tags)
        {
            Title = noteModel.Title;
            Description = noteModel.Description;
            AuthorId = user.UserId;
            AuthorName = user.Name;
            AuthorProfilePicturePath = user.ProfilePicturePath;
            Images = images;
            Score = score;
            Tags = tags;
        }

        public string Title { get; }
        public string Description { get; }
        public string AuthorId { get; }
        public string AuthorName { get; }
        public string AuthorProfilePicturePath { get; }
        public IEnumerable<ImageModel> Images { get; }
        public int Score { get; }
        public IEnumerable<string> Tags { get; }
    }
}
