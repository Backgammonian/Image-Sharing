using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data;
using MyWebApp.ViewModels;
using System.Diagnostics;

namespace MyWebApp.Controllers
{
    public sealed class HomeController : Controller
    {
        private readonly MyUrlHelper _urlHelper;

        public HomeController(MyUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View(new PrivacyViewModel()
            {
                Url = _urlHelper.GetCurrentUrl()
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}