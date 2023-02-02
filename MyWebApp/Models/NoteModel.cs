using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class NoteModel
    {
        [Key]
        public string NoteId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public UserModel? User { get; set; }
        public ICollection<NoteImageModel>? Images { get; set; }
        public ICollection<NoteThreadModel>? NoteThreads { get; set; }
    }
}