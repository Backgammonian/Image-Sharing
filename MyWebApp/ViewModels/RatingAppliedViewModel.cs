using MyWebApp.Models;

namespace MyWebApp.ViewModels
{
    public sealed class RatingAppliedViewModel
    {
        public RatingAppliedViewModel(string previousUrl)
        {
            PreviousUrl = previousUrl;
        }

        public string PreviousUrl { get; }
        public UserModel VotingUser { get; set; } = new UserModel();
        public NoteModel Note { get; set; } = new NoteModel();
        public RatingModel? Rating { get; set; }
    }
}
