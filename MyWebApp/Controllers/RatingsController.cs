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


            if (await _ratingsRepository.VoteUp(noteId))
            {
                var 
                return View();
            }

            
        }

        [HttpGet]
        [Route("Ratings/Downvote/{noteId}")]
        public async Task<IActionResult> Downvote(string noteId)
        {
            return View();
        }
    }
}
