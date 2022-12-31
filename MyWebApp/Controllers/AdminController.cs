using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyWebApp.Extensions;
using MyWebApp.Repository;
using MyWebApp.ViewModels;

namespace MyWebApp.Controllers
{
    public sealed class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly ThreadsRepository _threadsRepository;
        private readonly CredentialsRepository _credentialsRepository;

        public AdminController(ILogger<AdminController> logger,
            ThreadsRepository threadsRepository,
            CredentialsRepository credentialsRepository)
        {
            _logger = logger;
            _threadsRepository = threadsRepository;
            _credentialsRepository = credentialsRepository;
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
            if (!claims.IsAdmin())
            {
                return View(null);
            }

            var allThreads = await _threadsRepository.GetAllThreads();
            var createThreadVM = new CreateThreadViewModel()
            {
                NewThreadName = string.Empty,
                ExistingThreadNames = allThreads.Select(x => x.Thread)
            };

            return View(createThreadVM);
        }

        [HttpPost]
        [Route("Admin/CreateThread")]
        public async Task<IActionResult> CreateThread(CreateThreadViewModel createThreadVM)
        {
            var credentials = await _credentialsRepository.GetLoggedInUser();
            var claims = credentials.ClaimsPrincipal;
            if (!claims.IsAdmin() &&
                credentials.User != null)
            {
                ModelState.AddModelError(string.Empty, "You don't have the permission to create a new thread.");
                _logger.LogInformation($"(Admin/CreateThread) User {credentials.User.UserName} ({credentials.User.Id}) is trying to use the admin panel");

                return View(createThreadVM);
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Can't create a new thread.");

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

            ModelState.AddModelError(string.Empty, $"The thread '{createThreadVM.NewThreadName}' already exists (or something else went wrong).");

            return View(createThreadVM);
        }

        [HttpGet]
        [Route("Admin/DeleteThread")]
        public async Task<IActionResult> DeleteThread()
        {
            var credentials = await _credentialsRepository.GetLoggedInUser();
            var claims = credentials.ClaimsPrincipal;
            if (!claims.IsAdmin())
            {
                return View(null);
            }

            var availableThreads = await _threadsRepository.GetAllThreads();
            var selectableListItems = availableThreads.Select(x => new SelectListItem()
            {
                Value = x.Thread,
                Text = x.Thread,
            });

            var firstSelectableThread = selectableListItems.First();
            var deleteThreadVM = new DeleteThreadViewModel()
            {
                SelectedThreadName = firstSelectableThread != null ? 
                    firstSelectableThread.Value :
                    string.Empty,
                AvailableThreads = selectableListItems
            };

            return View(deleteThreadVM);
        }

        [HttpPost]
        [Route("Admin/DeleteThread")]
        public async Task<IActionResult> DeleteThread(DeleteThreadViewModel deleteThreadVM)
        {
            var credentials = await _credentialsRepository.GetLoggedInUser();
            var claims = credentials.ClaimsPrincipal;
            if (!claims.IsAdmin() &&
                credentials.User != null)
            {
                ModelState.AddModelError(string.Empty, "You don't have the permission to delete any thread");
                _logger.LogInformation($"(Admin/DeleteThread) User {credentials.User.UserName} ({credentials.User.Id}) is trying to use the admin panel");

                return View(deleteThreadVM);
            }

            if (await _threadsRepository.Delete(deleteThreadVM))
            {
                _logger.LogInformation($"(Admin/DeleteThread) Thread deleted: '{deleteThreadVM.SelectedThreadName}'");

                return RedirectToAction("Index");
            }
            else
            {
                _logger.LogInformation($"(Admin/DeleteThread) Can't delete the thread: '{deleteThreadVM.SelectedThreadName}'");
            }

            ModelState.AddModelError(string.Empty, $"Can't delete thread: '{deleteThreadVM.SelectedThreadName}'");

            return View(deleteThreadVM);
        }
    }
}
