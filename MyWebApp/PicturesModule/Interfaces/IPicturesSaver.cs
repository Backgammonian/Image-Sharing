namespace MyWebApp.PicturesModule.Interfaces
{
    public interface IPicturesSaver
    {
        Task SaveFile(IFormFile file, string destinationPath);
        void SaveFile(string sourcePath, string destinationPath);
    }
}
