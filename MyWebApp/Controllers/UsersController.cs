using Microsoft.AspNetCore.Mvc;
using MyWebApp.Repository.Interfaces;
using MyWebApp.ViewModels;

namespace MyWebApp.Controllers
{
    public sealed class UsersController : Controller
    {
        private readonly IUsersRepository _usersRepository;

        public UsersController(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        [HttpGet]
        [Route("Users")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
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

            var users = await _usersRepository.GetUsers((page - 1) * pageSize, pageSize);
            var count = await _usersRepository.GetCount();

            return View(new AllUsersViewModel()
            {
                Users = users,
                PagingViewModel = new PagingViewModel()
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = count,
                    TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                }
            });
        }

        [HttpGet]
        [Route("Users/Details/{userId}")]
        public async Task<IActionResult> Details(string userId)
        {
            return View(await _usersRepository.GetUserDetails(userId));
        }

        [HttpGet]
        [Route("Users/Notes/{userId}")]
        public async Task<IActionResult> Notes(string userId, int page = 1, int pageSize = 6)
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

            var userNotes = await _usersRepository.GetUserNotes(userId, (page - 1) * pageSize, pageSize);
            if (userNotes != null)
            {
                var count = await _usersRepository.GetCountOfUserNotes(userId);
                userNotes.PagingViewModel = new PagingViewModel()
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = count,
                    TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                };

                return View(userNotes);
            }

            return View(null);
        }
    }
}
