using Microsoft.AspNetCore.Mvc;
using MyWebApp.Extensions;
using MyWebApp.Repository;
using MyWebApp.ViewModels;

namespace MyWebApp.Controllers
{
    public sealed class AdminController : Controller
    {
        private readonly ThreadsRepository _threadsRepository;
        private readonly CredentialsRepository _credentialsRepository;

        public AdminController(ThreadsRepository threadsRepository,
            CredentialsRepository credentialsRepository)
        {
            _threadsRepository = threadsRepository;
            _credentialsRepository = credentialsRepository;
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
            if (!claims.IsAdmin())
            {
                ModelState.AddModelError(string.Empty, "You don't have the permission to create a new thread");

                return View("Create", createThreadVM);
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Can't create a new thread");

                return View("Create", createThreadVM);
            }

            if (await _threadsRepository.Create(createThreadVM))
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, $"Thread {createThreadVM.NewThreadName} already exists");

            return View("Create", createThreadVM);
        }

        [HttpGet]
        [Route("Admin/DeleteThread")]
        public async Task<IActionResult> DeleteThread()
        {

            return View();
        }

        [HttpPost]
        [Route("Admin/DeleteThread")]
        public async Task<IActionResult> DeleteThread(DeleteThreadViewModel deleteThreadVM)
        {

            return View();
        }
    }
}
