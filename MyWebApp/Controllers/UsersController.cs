using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.TableModels;

namespace MyWebApp.Controllers
{
    public sealed class UsersController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public UsersController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("Users/Info/{userId}")]
        public async Task<IActionResult> Info(string userId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpGet]
        [Route("Users/Notes/{userId}")]
        public async Task<IActionResult> Notes(string userId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null)
            {
                return NotFound();
            }

            var notes = await _dbContext.Notes.Where(x => x.UserId == userId).ToListAsync();
            var notesDetails = new List<NoteDetailsModel>();
            foreach (var note in notes)
            {
                var images = await _dbContext.Images.Where(x => x.NoteId == note.NoteId).ToListAsync();
                var ratings = await _dbContext.Ratings.Where(x => x.NoteId == note.NoteId).ToListAsync();
                var score = 0;
                foreach (var rating in ratings)
                {
                    score += rating.Score;
                }
                var noteTags = await _dbContext.TagsForNotes.Where(x => x.NoteId == note.NoteId).Select(x => x.Tag).ToListAsync();
                notesDetails.Add(new NoteDetailsModel(note, images, user, score, noteTags));
            }
            var result = (user, notesDetails);

            return View(result);
        }

        [HttpGet]
        [Route("Users/Ratings/{userId}")]
        public async Task<IActionResult> Ratings(string userId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null)
            {
                return NotFound();
            }

            var ratings = await _dbContext.Ratings.Where(x => x.UserId == userId).ToListAsync();
            var ratingsAndNotes = new List<(RatingModel, NoteModel)>();
            foreach (var rating in ratings)
            {
                var note = await _dbContext.Notes.FirstOrDefaultAsync(x => x.NoteId == rating.NoteId);
                if (note == null)
                {
                    continue;
                }

                ratingsAndNotes.Add((rating, note));
            }
            var result = (user, ratingsAndNotes);

            return View(result);
        }
    }
}
