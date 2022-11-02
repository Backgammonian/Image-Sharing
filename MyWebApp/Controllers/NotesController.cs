using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.TableModels;

namespace MyWebApp.Controllers
{
    public sealed class NotesController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public NotesController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("Notes")]
        public async Task<IActionResult> Index()
        {
            var result = new List<(NoteModel, int, List<string>)>();
            var notes = await _dbContext.Notes.ToListAsync();
            foreach (var note in notes)
            {
                var noteScore = await _dbContext.Ratings.Where(x => x.NoteId == note.NoteId).SumAsync(x => x.Score);
                var noteTags = await _dbContext.TagsForNotes.Where(x => x.NoteId == note.NoteId).Select(x => x.Tag).ToListAsync();
                result.Add((note, noteScore, noteTags));
            }

            return View(result);
        }

        [HttpGet]
        [Route("Notes/Details/{noteId}")]
        public async Task<IActionResult> Details(string noteId)
        {
            var note = await _dbContext.Notes.FirstOrDefaultAsync(x => x.NoteId == noteId);
            if (note == null)
            {
                return NotFound();
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == note.UserId);
            if (user == null)
            {
                return NotFound();
            }

            var images = await _dbContext.Images.Where(x => x.NoteId == noteId).ToListAsync();
            var ratings = await _dbContext.Ratings.Where(x => x.NoteId == noteId).ToListAsync();
            var score = 0;
            foreach (var rating in ratings)
            {
                score += rating.Score;
            }
            var tags = await _dbContext.TagsForNotes.Where(x => x.NoteId == noteId).Select(x => x.Tag).ToListAsync();
            var noteDetails = new NoteDetailsModel(note, images, user, score, tags);

            return View(noteDetails);
        }
    }
}
