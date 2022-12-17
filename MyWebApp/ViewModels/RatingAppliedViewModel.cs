using MyWebApp.Models;

namespace MyWebApp.ViewModels
{
    public sealed class RatingAppliedViewModel
    {
        public UserModel? VotingUser { get; set; }
        public RatingModel? Rating { get; set; }
        public NoteDetailsViewModel? NoteDetails { get; set; }
    }
}
