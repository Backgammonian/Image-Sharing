using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class NoteImageModel
    {
        [Key]
        public string ImageId { get; set; } = string.Empty;
        public string ImageFileName { get; set; } = string.Empty;
        public DateTimeOffset UploadTime { get; set; }
        public string NoteId { get; set; } = string.Empty;
        public NoteModel? Note { get; set; }
    }
}
