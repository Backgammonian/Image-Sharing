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
        public async Task<IActionResult> GetByThread(string thread, int page = 1, int pageSize = 6)
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

            var notesFromThread = await _threadsRepository.GetByThread(thread, (page - 1) * pageSize, pageSize);
            var count = await _threadsRepository.GetCountOfNotesFromThread(thread);
            notesFromThread.PagingViewModel = new PagingViewModel()
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = count,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
            };

            return View(notesFromThread);
        }
    }
}
