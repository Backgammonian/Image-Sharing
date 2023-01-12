using MyWebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.ViewModels
{
    public class EditNoteViewModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public string NoteId { get; set; } = string.Empty;
        public string SelectedThread { get; set; } = string.Empty;
        public IEnumerable<IFormFile> Images { get; set; } = Enumerable.Empty<IFormFile>();
    }
}