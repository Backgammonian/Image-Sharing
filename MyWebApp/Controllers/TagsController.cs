using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.TableModels;

namespace MyWebApp.Controllers
{
    public class TagsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public TagsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("Tags/GetByTag/{tag}")]
        public async Task<IActionResult> GetByTag(string tag)
        {
            var idsOfNotesWithTag = await _dbContext.TagsForNotes.Where(x => x.Tag == tag).ToListAsync();
            var notes = new List<NoteModel>();
            for (int i = 0; i < idsOfNotesWithTag.Count; i++)
            {
                var note = await _dbContext.Notes.FirstOrDefaultAsync(x => x.NoteId == idsOfNotesWithTag[i].NoteId);
                if (note != null)
                {
                    notes.Add(note);
                }
            }

            var result = new List<(NoteModel, int, List<string>)>();
            foreach (var note in notes)
            {
                var noteScore = await _dbContext.Ratings.Where(x => x.NoteId == note.NoteId).SumAsync(x => x.Score);
                var noteTags = await _dbContext.TagsForNotes.Where(x => x.NoteId == note.NoteId).Select(x => x.Tag).ToListAsync();
                result.Add((note, noteScore, noteTags));
            }

            return View((tag, result));
        }
    }
}
