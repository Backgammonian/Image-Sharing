namespace MyWebApp.PicturesModule.Interfaces
{
    public interface IImagePathHelper
    {
        string GetPath(string imageFileName);
        string GetDefaultImagePath();
        string GetHref(string imageFileName);
        string GetDefaultImageHref();
    }
}
