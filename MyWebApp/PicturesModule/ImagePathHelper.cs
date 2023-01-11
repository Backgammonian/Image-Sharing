using MyWebApp.Data;
using MyWebApp.PicturesModule.Interfaces;

namespace MyWebApp.PicturesModule
{
    public sealed class ImagePathHelper : IImagePathHelper
    {
        public string GetPath(string imageFileName)
        {
            return $"~/images/{imageFileName}";
        }

        public string GetDefaultImagePath()
        {
            return $"~/images/{Constants.DefaultImageName}";
        }

        public string GetHref(string imageFileName)
        {
            return $"/images/{imageFileName}";
        }

        public string GetDefaultImageHref()
        {
            return $"/images/{Constants.DefaultImageName}";
        }
    }
}