namespace MyWebApp.ViewModels
{
    public sealed class NoteSummariesListViewModel
    {
        public IEnumerable<NoteSummaryViewModel> NotesSummaries { get; set; } = Enumerable.Empty<NoteSummaryViewModel>();
    }
}
