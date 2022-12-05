using MyWebApp.Models;

namespace MyWebApp.ViewModels
{
    public sealed class NoteRatingViewModel
    {
        public NoteDetailsViewModel NoteDetails { get; set; } = new NoteDetailsViewModel();
        public RatingModel Rating { get; set; } = new RatingModel();
    }
}