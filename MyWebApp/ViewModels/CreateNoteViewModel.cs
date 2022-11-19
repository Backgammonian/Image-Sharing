using MyWebApp.Data;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.ViewModels
{
    public sealed class CreateNoteViewModel
    {
        public string NoteId { get; set; } = RandomGenerator.GetRandomId();
        public string UserId { get; set; } = string.Empty;
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}