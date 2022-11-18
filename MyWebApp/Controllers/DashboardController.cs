using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data;

namespace MyWebApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public DashboardController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("Dashboard")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
