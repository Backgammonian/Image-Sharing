using MyWebApp.Data;

namespace MyWebApp.Extensions
{
    public static class StringExtensions
    {
        public static bool IsEmpty(this string? str)
        {
            return str == null ||
                str.Length == 0 ||
                string.IsNullOrEmpty(str) ||
                string.IsNullOrWhiteSpace(str);
        }

        public static string Minify(this string? str, int maxLength = Constants.MinifiedTitleLength)
        {
            if (str == null ||
                maxLength <= 0) 
            {
                return string.Empty;
            }

            if (str.Length > maxLength)
            {
                return str;   
            }

            return $"{str.Substring(0, maxLength)}...";
        }
    }
}