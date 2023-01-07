using System.ComponentModel.DataAnnotations;

namespace MyWebApp.ViewModels
{
    public sealed class EditPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Display(Name = "Confirm new password")]
        [Required(ErrorMessage = "New password confirmation is required")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "New password doesn't match")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
