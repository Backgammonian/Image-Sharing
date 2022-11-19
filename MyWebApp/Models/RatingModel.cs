using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public sealed class RatingModel
    {
        [Key]
        public string RatingId { get; set; } = string.Empty;
        [ForeignKey(nameof(UserModel))]
        public string UserId { get; set; } = string.Empty;
        [ForeignKey(nameof(NoteModel))]
        public string NoteId { get; set; } = string.Empty;
        public int Score { get; set; }
    }
}
