using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class PreviousNoteImageModel
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        [ForeignKey(nameof(NoteImageModel))]
        public string FormerImageId { get; set; } = string.Empty;

        [ForeignKey(nameof(NoteModel))]
        public string NoteId { get; set; } = string.Empty;

        public string ImageFileName { get; set; } = string.Empty;
    }
}
