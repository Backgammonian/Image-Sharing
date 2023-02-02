namespace MyWebApp.Models
{
    public class NoteThreadModel
    {
        public string ThreadId { get; set; } = string.Empty;
        public string NoteId { get; set; } = string.Empty;
        public ThreadModel? Thread { get; set; }
        public NoteModel? Note { get; set; }
    }
}