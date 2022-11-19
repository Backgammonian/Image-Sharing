using Microsoft.AspNetCore.Mvc;
using MyWebApp.Repository;

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
        public async Task<IActionResult> Notes(string userId)
        {
            return View(await _usersRepository.GetUserNotes(userId));
        }

        [HttpGet]
        [Route("Users/Ratings/{userId}")]
        public async Task<IActionResult> Ratings(string userId)
        {
            return View(await _usersRepository.GetUserRatings(userId));
        }
    }
}
