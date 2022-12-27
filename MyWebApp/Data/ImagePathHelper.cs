namespace MyWebApp.Data
{
    public static class ImagePathHelper
    {
        public static string GetPath(string imageFileName)
        {
            return $"~/images/{imageFileName}";
        }

        public static string GetDefaultImagePath()
        {
            return $"~/images/{PicturesLoader.DefaultImageName}";
        }

        public static string GetHref(string imageFileName)
        {
            return $"/images/{imageFileName}";
        }

        public static string GetDefaultImageHref()
        {
            return $"/images/{PicturesLoader.DefaultImageName}";
        }
    }
}