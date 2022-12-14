namespace MyWebApp.ViewModels
{
    public sealed class NotesFromThreadViewModel
    {
        public string Thread { get; set; } = string.Empty;
        public IEnumerable<NoteDetailsViewModel> NotesDetails { get; set; } = Enumerable.Empty<NoteDetailsViewModel>();
    }
}