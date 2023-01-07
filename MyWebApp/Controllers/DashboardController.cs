using Microsoft.AspNetCore.Mvc;
using MyWebApp.Localization;
using MyWebApp.Repository;
using MyWebApp.ViewModels;

namespace MyWebApp.Controllers
{
    public sealed class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly CredentialsRepository _credentialsRepository;
        private readonly DashboardRepository _dashboardRepository;
        private readonly LanguageService _languageService;

        public DashboardController(ILogger<DashboardController> logger,
            CredentialsRepository credentialsRepository,
            DashboardRepository dashboardRepository,
            LanguageService languageService)
        {
            _logger = logger;
            _credentialsRepository = credentialsRepository;
            _dashboardRepository = dashboardRepository;
            _languageService = languageService;
        }

        [HttpGet]
        [Route("Dashboard")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 6)
        {
            if (page < 1 ||
                pageSize < 1)
            {
                return RedirectToAction("ErrorWrongPage", "Error", new WrongPageViewModel()
                {
                    Page = page,
                    PageSize = pageSize
                });
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

            return View(null);
        }

        [HttpGet]
        [Route("Dashboard/EditUserProfile")]
        public async Task<IActionResult> EditUserProfile()
        {
            var credentials = await _credentialsRepository.GetLoggedInUser();
            var currentUser = credentials.User;

            if (currentUser == null)
            {
                return View("Error");
            }

            var editUserProfileViewModel = new EditUserProfileViewModel()
            {
                Status = currentUser.Status,
                UserName = currentUser.UserName
            };

            return View(editUserProfileViewModel);
        }

        [HttpPost]
        [Route("Dashboard/EditUserProfile")]
        public async Task<IActionResult> EditUserProfile(EditUserProfileViewModel editUserProfileVM)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = _languageService.GetKey("EditUserProfile_WrongInput");

                return View(editUserProfileVM);
            }

            var credentials = await _credentialsRepository.GetLoggedInUser(false);
            var currentUser = credentials.User;

            if (currentUser == null)
            {
                return View("Error");
            }

            if (await _dashboardRepository.Update(currentUser, editUserProfileVM))
            {
                _logger.LogInformation($"(Dashboard/EditUserProfile) User {currentUser.UserName} ({currentUser.Id}) updated his profile");

                return RedirectToAction("Index");
            }
            else
            {
                _logger.LogInformation($"(Dashboard/EditUserProfile) ⚠️ User {currentUser.UserName} ({currentUser.Id}) failed to update his profile");
            }

            TempData["Error"] = _languageService.GetKey("EditUserProfile_NoEditPermission");

            return View(editUserProfileVM);
        }

        [HttpGet]
        [Route("Dashboard/EditPassword")]
        public async Task<IActionResult> EditPassword()
        {
            var credentials = await _credentialsRepository.GetLoggedInUser();
            var currentUser = credentials.User;

            if (currentUser == null)
            {
                return View("Error");
            }

            _logger.LogInformation($"(Dashboard/EditUserProfile) User {currentUser.UserName} ({currentUser.Id}) is going to update his PASSWORD");

            return View(new EditPasswordViewModel());
        }

        [HttpPost]
        [Route("Dashboard/EditPassword")]
        public async Task<IActionResult> EditPassword(EditPasswordViewModel editPasswordVM)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = _languageService.GetKey("EditPassword_WrongInput");

                return View(editPasswordVM);
            }

            var credentials = await _credentialsRepository.GetLoggedInUser();
            var currentUser = credentials.User;

            if (currentUser == null)
            {
                return View("Error");
            }

            if (await _dashboardRepository.UpdatePassword(currentUser.Id, editPasswordVM))
            {
                _logger.LogInformation($"(Dashboard/EditUserProfile) User {currentUser.UserName} ({currentUser.Id}) updated his PASSWORD");

                return RedirectToAction("Index");
            }
            else
            {
                _logger.LogInformation($"(Dashboard/EditUserProfile) ⚠️ User {currentUser.UserName} ({currentUser.Id}) failed to update his PASSWORD");
            }

            TempData["Error"] = _languageService.GetKey("EditPassword_NoEditPermission");

            return View(editPasswordVM);
        }
    }
}