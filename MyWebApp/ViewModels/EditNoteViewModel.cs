namespace MyWebApp.ViewModels
{
    public class EditNoteViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}