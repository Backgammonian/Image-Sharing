using MyWebApp.TableModels;

namespace MyWebApp.Models
{
    public sealed class UserNotes
    {
        public UserNotes(UserModel user, UserImageModel profilePicture, IEnumerable<NoteDetails> notes)
        {
            User = user;
            ProfilePicture = profilePicture;
            Notes = notes;  
        }

        public UserModel User { get; }
        public UserImageModel ProfilePicture { get; }
        public IEnumerable<NoteDetails> Notes { get; }
    }
}
