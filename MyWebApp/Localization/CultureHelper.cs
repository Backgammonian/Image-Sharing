using MyWebApp.Localization.Interfaces;
using System.Globalization;

namespace MyWebApp.Localization
{
    public sealed class CultureHelper : ICultureHelper
    {
        public string GetCulture()
        {
            return CultureInfo.CurrentCulture.ToString();
        }
    }
}
