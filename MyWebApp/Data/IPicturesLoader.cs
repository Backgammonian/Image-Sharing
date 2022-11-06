using MyWebApp.TableModels;

namespace MyWebApp.Data
{
    public interface IPicturesLoader
    {
        Task<NoteImageModel> LoadNoteImage(IFormFile image, NoteModel note);
        Task<UserImageModel> LoadProfileImage(IFormFile image, UserModel user);
        List<NoteImageModel> LoadDemoNoteImages(List<NoteModel> notes);
        List<UserImageModel> LoadDemoProfileImages(List<UserModel> notes);
    }
}
