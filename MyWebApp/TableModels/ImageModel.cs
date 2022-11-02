using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.TableModels
{
    public sealed class ImageModel
    {
        [Key]
        public string ImageId { get; set; } = string.Empty;
        [ForeignKey(nameof(NoteModel))]
        public string NoteId { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
    }
}
