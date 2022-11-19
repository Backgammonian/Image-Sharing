using MyWebApp.Models;

namespace MyWebApp.ViewModels
{
    public sealed class NoteRatingViewModel
    {
        public NoteDetailsViewModel? NoteDetails { get; set; }
        public RatingModel? Rating { get; set; }
    }
}