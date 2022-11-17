namespace MyWebApp.ViewModels
{
    public sealed class EditNoteViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}