using System.Globalization;

namespace MyWebApp.Localization
{
    public static class CultureHelper
    {
        public static string GetCulture()
        {
            return CultureInfo.CurrentCulture.ToString();
        }
    }
}
