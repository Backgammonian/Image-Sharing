namespace MyWebApp.ViewModels
{
    public class NotesFromThreadViewModel
    {
        public string Thread { get; set; } = string.Empty;
        public IEnumerable<NoteDetailsViewModel> NotesDetails { get; set; } = Enumerable.Empty<NoteDetailsViewModel>();
        public PagingViewModel PagingViewModel { get; set; } = new PagingViewModel();
    }
}