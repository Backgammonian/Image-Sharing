using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyWebApp.Extensions;
using MyWebApp.Localization;
using MyWebApp.Repository;
using MyWebApp.ViewModels;

namespace MyWebApp.Controllers
{
    public sealed class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ThreadsRepository _threadsRepository;
        private readonly CredentialsRepository _credentialsRepository;
        private readonly LanguageService _languageService;

        public AdminController(ILogger<AdminController> logger,
            ThreadsRepository threadsRepository,
            CredentialsRepository credentialsRepository,
            LanguageService languageService)
        {
            _logger = logger;
            _threadsRepository = threadsRepository;
            _credentialsRepository = credentialsRepository;
            _languageService = languageService;
        }

        [HttpGet]
        [Route("Admin")]
        public async Task<IActionResult> Index()
        {
            var credentials = await _credentialsRepository.GetLoggedInUser();

            return View(credentials);
        }

        [HttpGet]
        [Route("Admin/CreateThread")]
        public async Task<IActionResult> CreateThread()
        {
            var credentials = await _credentialsRepository.GetLoggedInUser();
            var claims = credentials.ClaimsPrincipal;
            var user = credentials.User;

            if (user == null || 
                !claims.IsAdmin())
            {
                return View(null);
            }

            var createThreadVM = new CreateThreadViewModel()
            {
                NewThreadName = string.Empty
            };

            return View(createThreadVM);
        }

        [HttpPost]
        [Route("Admin/CreateThread")]
        public async Task<IActionResult> CreateThread(CreateThreadViewModel createThreadVM)
        {
            var credentials = await _credentialsRepository.GetLoggedInUser();
            var claims = credentials.ClaimsPrincipal;
            var user = credentials.User;

            if (user == null ||
                !claims.IsAdmin())
            {
                return View(null);
            }

            _logger.LogInformation($"(Admin/CreateThread) User {user.UserName} ({user.Id}) is using the admin panel to create a new thread called '{createThreadVM.NewThreadName}'");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = _languageService.GetKey("CreateThread_CantCreate");

                return View(createThreadVM);
            }

            if (await _threadsRepository.Create(createThreadVM))
            {
                _logger.LogInformation($"(Admin/CreateThread) New thread created: '{createThreadVM.NewThreadName}'");

                return RedirectToAction("Index");
            }
            else
            {
                _logger.LogInformation($"(Admin/CreateThread) Can't create a new thread: '{createThreadVM.NewThreadName}'");
            }

            TempData["Error"] = _languageService.GetKey("CreateThread_ThreadAlreadyExists");

            return View(createThreadVM);
        }

        [HttpGet]
        [Route("Admin/DeleteThread")]
        public async Task<IActionResult> DeleteThread()
        {
            var credentials = await _credentialsRepository.GetLoggedInUser();
            var claims = credentials.ClaimsPrincipal;
            var user = credentials.User;

            if (user == null ||
                !claims.IsAdmin())
            {
                return View(null);
            }

            return View(new DeleteThreadViewModel());
        }

        [HttpPost]
        [Route("Admin/DeleteThread")]
        public async Task<IActionResult> DeleteThread(DeleteThreadViewModel deleteThreadVM)
        {
            var credentials = await _credentialsRepository.GetLoggedInUser();
            var claims = credentials.ClaimsPrincipal;
            var user = credentials.User;

            if (user == null ||
                !claims.IsAdmin())
            {
                return View(null);
            }

            _logger.LogInformation($"(Admin/DeleteThread) User {user.UserName} ({user.Id}) is using the admin panel to delete the thread '{deleteThreadVM.SelectedThreadName}'");

            if (await _threadsRepository.Delete(deleteThreadVM))
            {
                _logger.LogInformation($"(Admin/DeleteThread) Thread deleted: '{deleteThreadVM.SelectedThreadName}'");

                return RedirectToAction("Index");
            }
            else
            {
                _logger.LogInformation($"(Admin/DeleteThread) Can't delete the thread: '{deleteThreadVM.SelectedThreadName}'");
            }

            TempData["Error"] = _languageService.GetKey("DeleteThread_CantDelete");

            return View(deleteThreadVM);
        }
    }
}
