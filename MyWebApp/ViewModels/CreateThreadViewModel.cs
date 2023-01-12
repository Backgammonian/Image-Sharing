using System.ComponentModel.DataAnnotations;

namespace MyWebApp.ViewModels
{
    public class CreateThreadViewModel
    {
        [Required]
        public string NewThreadName { get; set; } = string.Empty;
    }
}
