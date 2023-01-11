namespace MyWebApp.ViewModels
{
    public sealed class DeleteNoteViewModel
    {
        public string NoteId { get; set; } = string.Empty;
        public NoteDetailsViewModel? NoteDetails { get; set; }
    }
}
