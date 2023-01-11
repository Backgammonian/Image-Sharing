using Microsoft.Extensions.Localization;

namespace MyWebApp.Localization.Interfaces
{
    public interface ILanguageService
    {
        LocalizedString GetKey(string key);
    }
}