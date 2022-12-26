using Microsoft.AspNetCore.Mvc;
using MyWebApp.Repository;
using MyWebApp.ViewModels;

namespace MyWebApp.Controllers
{
    public sealed class UsersController : Controller
    {
        private readonly UsersRepository _usersRepository;

        public UsersController(UsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
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
                return NotFound();
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

            return NotFound();
        }
    }
}
