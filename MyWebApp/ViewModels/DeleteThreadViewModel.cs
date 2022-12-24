using System.ComponentModel.DataAnnotations;

namespace MyWebApp.ViewModels
{
    public sealed class DeleteThreadViewModel
    {
        [Required]
        public string ThreadName { get; set; } = string.Empty;
    }
}
