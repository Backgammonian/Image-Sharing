namespace MyWebApp.ViewModels
{
    public sealed class EditUserProfileViewModel
    {
        public string Status { get; set; } = string.Empty;
        public IFormFile? ProfileImage { get; set; }
    }
}
