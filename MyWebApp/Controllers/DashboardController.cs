using Microsoft.AspNetCore.Mvc;
using MyWebApp.Repository;
using MyWebApp.ViewModels;

namespace MyWebApp.Controllers
{
    public sealed class DashboardController : Controller
    {
        private readonly DashboardRepository _dashboardRepository;

        public DashboardController(DashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        [HttpGet]
        [Route("Dashboard")]
        public async Task<IActionResult> Index()
        {
            return View(new DashboardViewModel()
            {
                UserNotes = await _dashboardRepository.GetAllUserNotes(),
                UserRatings = await _dashboardRepository.GetAllUserRatings()
            });
        }
    }
}
