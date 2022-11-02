using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApp.TableModels
{
    public sealed class TagsForNotesModel
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        [ForeignKey(nameof(TagModel))]
        public string Tag { get; set; } = string.Empty;
        [ForeignKey(nameof(NoteModel))]
        public string NoteId { get; set; } = string.Empty;
    }
}
