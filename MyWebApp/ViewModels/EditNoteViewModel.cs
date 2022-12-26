using Microsoft.AspNetCore.Mvc.Rendering;
using MyWebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.ViewModels
{
    public sealed class EditNoteViewModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public IEnumerable<SelectListItem> AvailableThreads { get; set; } = Enumerable.Empty<SelectListItem>();
        public string SelectedThread { get; set; } = string.Empty;
        public IEnumerable<IFormFile> Images { get; set; } = Enumerable.Empty<IFormFile>();
        public IEnumerable<NoteImageModel> ExistingImages { get; set; } = Enumerable.Empty<NoteImageModel>();
    }
}