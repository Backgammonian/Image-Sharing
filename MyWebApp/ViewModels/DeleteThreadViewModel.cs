using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyWebApp.ViewModels
{
    public sealed class DeleteThreadViewModel
    {
        public string SelectedThreadName { get; set; } = string.Empty;
    }
}