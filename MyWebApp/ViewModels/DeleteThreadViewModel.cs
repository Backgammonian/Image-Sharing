using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyWebApp.ViewModels
{
    public class DeleteThreadViewModel
    {
        public string SelectedThreadName { get; set; } = string.Empty;
    }
}