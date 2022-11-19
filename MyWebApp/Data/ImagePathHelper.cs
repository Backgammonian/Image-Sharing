namespace MyWebApp.Data
{
    public static class ImagePathHelper
    {
        public static string GetPath(string imageFileName)
        {
            return $"~/images/{imageFileName}";
        }
    }
}
