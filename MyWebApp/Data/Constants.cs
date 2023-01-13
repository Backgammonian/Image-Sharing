namespace MyWebApp.Data
{
    public static class Constants
    {
        public static string Chars { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";

        public static int IdLength { get; } = 40;
        public static int MinifiedTitleLength { get; } = 15;

        public static string ImagesFolderName { get; } = "images";
        public static string DemoNoteImagesFolderName { get; } = "demo-note-images";
        public static string DemoProfileImagesFolderName { get; } = "demo-profile-images";
        public static string DefaultImageFolderName { get; } = "default-images";
        public static string DefaultImageNameWithouExtension { get; } = "default";
        public static string DefaultImageName { get; } = "default.jpg";

        public static string UnknownIP { get; } = "Unknown IP";

        public static string AllowedUserNameCharacters { get; } = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZйцукенгшщзхъфывапролджэячсмитьбюёЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИТЬБЮЁ0123456789.,-+()=";
    }
}
