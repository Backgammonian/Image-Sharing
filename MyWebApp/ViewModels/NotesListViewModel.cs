using MyWebApp.Models;

namespace MyWebApp.ViewModels
{
    public sealed class NotesListViewModel
    {
        public IEnumerable<NoteSummaryViewModel> NoteSummaries { get; set; } = Enumerable.Empty<NoteSummaryViewModel>();
    }
}
