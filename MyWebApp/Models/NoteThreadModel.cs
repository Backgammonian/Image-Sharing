using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.Models
{
    public sealed class NoteThreadModel
    {
        [Key]
        public string Id { get; set; } = string.Empty;


        [ForeignKey(nameof(ThreadModel))]
        public string Thread { get; set; } = string.Empty;


        [ForeignKey(nameof(NoteModel))]
        public string NoteId { get; set; } = string.Empty;
    }
}
