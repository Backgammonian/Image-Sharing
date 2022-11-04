using MyWebApp.Data;

namespace MyWebApp.Models
{
    public sealed class CreateNoteViewModel
    {
        public string NoteId { get; set; } = RandomGenerator.GetRandomId();
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}