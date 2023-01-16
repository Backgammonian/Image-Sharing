using Microsoft.AspNetCore.Mvc;
using MyWebApp.ViewModels;

namespace MyWebApp.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Error500()
        {
            return View();
        }

        public IActionResult Error404()
        {
            return View();
        }

        public IActionResult ErrorWrongPage(WrongPageViewModel wrongPageVM)
        {
            return View(wrongPageVM);
        }

        public IActionResult ErrorNoAuthorization()
        {
            return View();
        }

        public IActionResult ErrorNoAuthentication()
        {
            return View();
        }
    }
}
