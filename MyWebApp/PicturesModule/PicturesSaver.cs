using MyWebApp.PicturesModule.Interfaces;

namespace MyWebApp.PicturesModule
{
    public sealed class PicturesSaver : IPicturesSaver
    {
        public async Task SaveFile(IFormFile file, string destinationPath)
        {
            using var stream = new FileStream(destinationPath, FileMode.Create);
            await file.CopyToAsync(stream);
        }

        public void SaveFile(string sourcePath, string destinationPath)
        {
            File.Copy(sourcePath, destinationPath, true);
        }
    }
}
