namespace MyWebApp.ViewModels
{
    public sealed class NotesListViewModel
    {
        public IEnumerable<NoteDetailsViewModel> NotesDetails { get; set; } = Enumerable.Empty<NoteDetailsViewModel>();
    }
}
