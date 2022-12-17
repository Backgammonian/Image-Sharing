using Microsoft.AspNetCore.Mvc;
using MyWebApp.Repository;

namespace MyWebApp.Controllers
{
    public sealed class RatingsController : Controller
    {
        private readonly RatingsRepository _ratingsRepository;

        public RatingsController(RatingsRepository ratingsRepository)
        {
            _ratingsRepository = ratingsRepository;
        }

        [HttpGet]
        [Route("Ratings/Upvote/{noteId}")]
        public async Task<IActionResult> Upvote(string noteId)
        {
            var result = await _ratingsRepository.VoteUp(noteId);
            if (result != null)
            {
                return View();
            }

            return View();
        }

        [HttpGet]
        [Route("Ratings/Downvote/{noteId}")]
        public async Task<IActionResult> Downvote(string noteId)
        {
            var result = await _ratingsRepository.VoteDown(noteId);
            if (result != null)
            {
                return View();
            }

            return View();
        }
    }
}
