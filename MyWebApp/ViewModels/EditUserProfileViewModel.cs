namespace MyWebApp.ViewModels
{
    public sealed class EditUserProfileViewModel
    {
        public string Status { get; set; } = string.Empty;
        public IFormFile? NewProfilePicture { get; set; }
    }
}
