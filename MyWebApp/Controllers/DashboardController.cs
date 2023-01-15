using Microsoft.AspNetCore.Mvc;
using MyWebApp.Localization.Interfaces;
using MyWebApp.Repository.Interfaces;
using MyWebApp.ViewModels;

namespace MyWebApp.Controllers
{
    public sealed class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly ILanguageService _languageService;
        private readonly ICredentialsRepository _credentialsRepository;
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardController(ILogger<DashboardController> logger,
            ILanguageService languageService,
            ICredentialsRepository credentialsRepository,
            IDashboardRepository dashboardRepository)
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
            var credentialsVM = await _credentialsRepository.GetLoggedInUser();
            var credentials = credentialsVM.Credentials;
            if (credentials != null &&
                credentials.IsNotAuthenticated())
            {
                return RedirectToAction("ErrorNoAuthentication", "Error");
            }

            var user = credentialsVM.User;
            if (user == null)
            {
                return RedirectToAction("ErrorNoAuthentication", "Error");
            }

            if (page < 1 ||
                pageSize < 1)
            {
                return RedirectToAction("ErrorWrongPage", "Error", new WrongPageViewModel()
                {
                    Page = page,
                    PageSize = pageSize
                });
            }

            var dashboardVM = await _dashboardRepository.GetDashboard(user, (page - 1) * pageSize, pageSize);
            var count = await _dashboardRepository.GetNotesCount(user);
            dashboardVM.PagingViewModel = new PagingViewModel()
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = count,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
            };

            return View(dashboardVM);
        }

        [HttpGet]
        [Route("Dashboard/EditUserProfile")]
        public async Task<IActionResult> EditUserProfile()
        {
            var credentialsVM = await _credentialsRepository.GetLoggedInUser();
            var credentials = credentialsVM.Credentials;
            if (credentials != null &&
                credentials.IsNotAuthenticated())
            {
                return RedirectToAction("ErrorNoAuthentication", "Error");
            }

            var user = credentialsVM.User;
            if (user == null)
            {
                return RedirectToAction("ErrorNoAuthentication", "Error");
            }

            var editUserProfileViewModel = new EditUserProfileViewModel()
            {
                UserId = user.Id,
                Status = user.Status,
                UserName = user.UserName
            };

            return View(editUserProfileViewModel);
        }

        [HttpPost]
        [Route("Dashboard/EditUserProfile")]
        public async Task<IActionResult> EditUserProfile(EditUserProfileViewModel editUserProfileVM)
        {
            var credentialsVM = await _credentialsRepository.GetLoggedInUser(false);
            var credentials = credentialsVM.Credentials;
            if (credentials != null &&
                credentials.IsNotAuthenticated())
            {
                return RedirectToAction("ErrorNoAuthentication", "Error");
            }

            var user = credentialsVM.User;
            if (user == null)
            {
                return RedirectToAction("ErrorNoAuthentication", "Error");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = _languageService.GetKey("EditUserProfile_WrongInput");

                return View(editUserProfileVM);
            }

            if (await _dashboardRepository.Update(user, editUserProfileVM))
            {
                _logger.LogInformation($"(Dashboard/EditUserProfile) User {user.UserName} ({user.Id}) updated his profile");

                return RedirectToAction("Index");
            }
            else
            {
                _logger.LogInformation($"(Dashboard/EditUserProfile) ⚠️ User {user.UserName} ({user.Id}) failed to update his profile");
            }

            TempData["Error"] = _languageService.GetKey("EditUserProfile_NoEditPermission");

            return View(editUserProfileVM);
        }

        [HttpGet]
        [Route("Dashboard/EditPassword")]
        public async Task<IActionResult> EditPassword()
        {
            var credentialsVM = await _credentialsRepository.GetLoggedInUser();
            var credentials = credentialsVM.Credentials;
            if (credentials != null &&
                credentials.IsNotAuthenticated())
            {
                return RedirectToAction("ErrorNoAuthentication", "Error");
            }

            var user = credentialsVM.User;
            if (user == null)
            {
                return RedirectToAction("ErrorNoAuthentication", "Error");
            }

            _logger.LogInformation($"(Dashboard/EditUserProfile) User {user.UserName} ({user.Id}) is going to update his PASSWORD");

            return View(new EditPasswordViewModel());
        }

        [HttpPost]
        [Route("Dashboard/EditPassword")]
        public async Task<IActionResult> EditPassword(EditPasswordViewModel editPasswordVM)
        {
            var credentialsVM = await _credentialsRepository.GetLoggedInUser();
            var credentials = credentialsVM.Credentials;
            if (credentials != null &&
                credentials.IsNotAuthenticated())
            {
                return RedirectToAction("ErrorNoAuthentication", "Error");
            }

            var user = credentialsVM.User;
            if (user == null)
            {
                return RedirectToAction("ErrorNoAuthentication", "Error");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = _languageService.GetKey("EditPassword_WrongInput");

                return View(editPasswordVM);
            }

            if (await _dashboardRepository.UpdatePassword(user.Id, editPasswordVM))
            {
                _logger.LogInformation($"(Dashboard/EditUserProfile) User {user.UserName} ({user.Id}) updated his PASSWORD");

                return RedirectToAction("Index");
            }
            else
            {
                _logger.LogInformation($"(Dashboard/EditUserProfile) ⚠️ User {user.UserName} ({user.Id}) failed to update his PASSWORD");
            }

            TempData["Error"] = _languageService.GetKey("EditPassword_NoEditPermission");

            return View(editPasswordVM);
        }
    }
}