using System.ComponentModel.DataAnnotations;

namespace MyWebApp.ViewModels
{
    public sealed class LoginViewModel
    {
        [Required(ErrorMessage = "Email address is required")]
        [Display(Name = "Email address")]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
