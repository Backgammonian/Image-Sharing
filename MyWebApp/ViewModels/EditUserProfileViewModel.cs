namespace MyWebApp.ViewModels
{
    public class EditUserProfileViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public IFormFile? NewProfilePicture { get; set; }
    }
}
