using Microsoft.AspNetCore.Mvc;
using MyWebApp.ViewModels;
using System.Diagnostics;

namespace MyWebApp.Controllers
{
    public sealed class HomeController : Controller
    {
        [HttpGet]
        [Route("Home")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("Home/Privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
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