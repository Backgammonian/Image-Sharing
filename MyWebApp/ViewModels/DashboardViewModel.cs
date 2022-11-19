namespace MyWebApp.ViewModels
{
    public sealed class DashboardViewModel
    {
        public IEnumerable<NoteDetailsViewModel> UserNotes { get; set; } = Enumerable.Empty<NoteDetailsViewModel>();
        public UserRatingsViewModel? UserRatings { get; set; }
    }
}
