using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public sealed class NoteModel
    {
        [Key]
        public string NoteId { get; set; } = string.Empty;
        [ForeignKey(nameof(UserModel))]
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
