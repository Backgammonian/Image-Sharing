namespace MyWebApp.ViewModels
{
    public sealed class TaggedNotesViewModel
    {
        public string Tag { get; set; } = string.Empty;
        public IEnumerable<NoteDetailsViewModel> TaggedNotesDetails { get; set; } = Enumerable.Empty<NoteDetailsViewModel>();
    }
}