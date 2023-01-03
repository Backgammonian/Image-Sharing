namespace MyWebApp.PicturesModule
{
    public sealed class ImagePathHelper
    {
        public string GetPath(string imageFileName)
        {
            return $"~/images/{imageFileName}";
        }

        public string GetDefaultImagePath()
        {
            return $"~/images/{PicturesLoader.DefaultImageName}";
        }

        public string GetHref(string imageFileName)
        {
            return $"/images/{imageFileName}";
        }

        public string GetDefaultImageHref()
        {
            return $"/images/{PicturesLoader.DefaultImageName}";
        }
    }
}