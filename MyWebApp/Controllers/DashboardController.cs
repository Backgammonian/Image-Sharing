using Microsoft.AspNetCore.Mvc;
using MyWebApp.Data;
using MyWebApp.Repository;
using MyWebApp.ViewModels;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace MyWebApp.Controllers
{
    public sealed class DashboardController : Controller
    {
        private readonly CredentialsRepository _credentialsRepository;
        private readonly DashboardRepository _dashboardRepository;

        public DashboardController(CredentialsRepository credentialsRepository,
            DashboardRepository dashboardRepository)
        {
            _credentialsRepository = credentialsRepository;
            _dashboardRepository = dashboardRepository;
        }

        [HttpGet]
        [Route("Dashboard")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 6)
        {
            if (page < 1 ||
                pageSize < 1)
            {
                return NotFound();
            }

            var dashboardVM = await _dashboardRepository.GetDashboard((page - 1) * pageSize, pageSize);
            if (dashboardVM != null)
            {
                var count = await _dashboardRepository.GetNotesCount();
                dashboardVM.PagingViewModel = new PagingViewModel()
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = count,
                    TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                };

                return View(dashboardVM);
            }

            return View(dashboardVM);
        }

        [HttpGet]
        [Route("Dashboard/EditUserProfile")]
        public async Task<IActionResult> EditUserProfile()
        {
            var credentials = await _credentialsRepository.GetLoggedInUser(true);
            var currentUser = credentials.User;

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

            var credentials = await _credentialsRepository.GetLoggedInUser(false);
            var currentUser = credentials.User;

            if (currentUser == null)
            {
                return View("Error");
            }

            if (await _dashboardRepository.Update(currentUser, editUserProfileVM))
            {
                return RedirectToAction("Index");
            }
            
            ModelState.AddModelError(string.Empty, "You have no permission to edit this profile.");

            return View("Edit", editUserProfileVM);
        }
    }
}
