using Microsoft.AspNetCore.Mvc;
using MyWebApp.Repository.Interfaces;

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
        [Route("Users/Info/{userId}")]
        public async Task<IActionResult> Info(string userId)
        {
            return View(await _usersRepository.GetUserInfo(userId));
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
