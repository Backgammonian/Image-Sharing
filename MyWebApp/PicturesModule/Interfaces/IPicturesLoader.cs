using MyWebApp.Models;

namespace MyWebApp.PicturesModule.Interfaces
{
    public interface IPicturesLoader
    {
        string GetNewFileName(string filePath);
        string GetNewFileName(IFormFile file);
        void EnsureFolderIsCreated();
        Task<NoteImageModel> LoadNoteImage(IFormFile image, NoteModel note);
        Task<UserImageModel> LoadProfileImage(IFormFile image, UserModel user);
        void LoadDefaultImage();
        UserImageModel GetDefaultProfileImage();
        NoteImageModel GetDefaultNoteImage();
        List<NoteImageModel> LoadDemoNoteImages(NoteModel[] notes);
        List<UserImageModel> LoadDemoProfileImages(UserModel[] users);
        Task<UserImageModel> GetUserCurrentProfilePicture(UserModel? user);
    }
}
