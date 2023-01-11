using Microsoft.Extensions.Localization;
using MyWebApp.Localization.Interfaces;
using System.Reflection;

namespace MyWebApp.Localization
{
    public sealed class LanguageService : ILanguageService
    {
        private readonly IStringLocalizer _localizer;

        public LanguageService(IStringLocalizerFactory factory)
        {
            var type = typeof(SharedResource);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
            _localizer = factory.Create("SharedResource", assemblyName.Name);
        }

        public LocalizedString GetKey(string key)
        {
            return _localizer[key];
        }
    }
}
