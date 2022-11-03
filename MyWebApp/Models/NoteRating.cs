using MyWebApp.TableModels;

namespace MyWebApp.Models
{
    public sealed class NoteRating
    {
        public NoteRating(NoteModel note, RatingModel rating)
        {
            Note = note;
            Rating = rating;
        }

        public NoteModel Note { get; }
        public RatingModel Rating { get; }
    }
}
