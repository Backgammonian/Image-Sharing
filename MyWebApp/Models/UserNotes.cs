using MyWebApp.TableModels;

namespace MyWebApp.Models
{
    public sealed class UserNotes
    {
        public UserNotes(UserModel user, IEnumerable<NoteDetails> notes)
        {
            User = user;
            Notes = notes;  
        }

        public UserModel User { get; }
        public IEnumerable<NoteDetails> Notes { get; }
    }
}
