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
    }
}
