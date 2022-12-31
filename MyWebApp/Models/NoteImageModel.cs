using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public sealed class NoteImageModel
    {
        [Key]
        public string ImageId { get; set; } = string.Empty;

        [ForeignKey(nameof(NoteModel))]
        public string NoteId { get; set; } = string.Empty;

        public string ImageFileName { get; set; } = string.Empty;
        public DateTimeOffset UploadTime { get; set; }
    }
}
