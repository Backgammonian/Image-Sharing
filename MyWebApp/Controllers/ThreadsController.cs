using Microsoft.AspNetCore.Mvc;
using MyWebApp.Repository;
using MyWebApp.ViewModels;

namespace MyWebApp.Controllers
{
    public sealed class ThreadsController : Controller
    {
        private readonly ThreadsRepository _threadsRepository;

        public ThreadsController(ThreadsRepository threadsRepository)
        {
            _threadsRepository = threadsRepository;
        }

        [HttpGet]
        [Route("Threads")]
        public async Task<IActionResult> Index()
        {
            return View(new AvailableThreadsViewModel()
            {
                Threads = await _threadsRepository.GetAllThreads()
            });
        }

        [HttpGet]
        [Route("Threads/GetByThread/{thread}")]
        public async Task<IActionResult> GetByThread(string thread)
        {
            return View(await _threadsRepository.GetByThread(thread));
        }
    }
}
