using MyWebApp.TableModels;

namespace MyWebApp.Models
{
    public sealed class UserRatings
    {
        public UserRatings(UserModel user, IEnumerable<NoteRating> ratingsOfNotes)
        {
            User = user;
            RatingsOfNotes = ratingsOfNotes;
        }

        public UserModel User { get; }
        public IEnumerable<NoteRating> RatingsOfNotes { get; }
    }
}
