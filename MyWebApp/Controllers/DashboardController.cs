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
            return View(await _dashboardRepository.GetDashboard());
        }

        [HttpGet]
        [Route("Dashboard/EditUserProfile")]
        public async Task<IActionResult> EditUserProfile()
        {
            var currentUser = await _dashboardRepository.GetCurrentUser();
            if (currentUser == null)
            {
                return View("Error");
            }

            var editUserProfileViewModel = new EditUserProfileViewModel()
            {
                Status = currentUser.Status
            };

            return View(editUserProfileViewModel);
        }

        [HttpPost]
        [Route("Dashboard/EditUserProfile")]
        public async Task<IActionResult> EditUserProfile(EditUserProfileViewModel editUserProfileVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Failed to edit the profile.");
                return View("EditUserProfile", editUserProfileVM);
            }

            var currentUser = await _dashboardRepository.GetCurrentUser();
            if (currentUser == null)
            {
                return View("Error");
            }

            if (await _dashboardRepository.Update(editUserProfileVM))
            {
                return RedirectToAction("Index");
            }
            
            ModelState.AddModelError(string.Empty, "You have no permission to edit this profile.");
            return View("Edit", editUserProfileVM);
        }
    }
}
