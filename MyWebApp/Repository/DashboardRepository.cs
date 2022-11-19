using Microsoft.AspNet.Identity;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.Repository.Interfaces;

namespace MyWebApp.Repository
{
    public sealed class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _contextAccessor;

        public DashboardRepository(ApplicationDbContext dbContext, IHttpContextAccessor contextAccessor)
        {
            _dbContext = dbContext;
            _contextAccessor = contextAccessor;
        }

        public async Task<IEnumerable<NoteDetails>> GetAllUserNotes()
        {
            var currentUser = _contextAccessor.HttpContext?.User;
            if (currentUser == null)
            {
                return Enumerable.Empty<NoteDetails>();
            }

            var userId = currentUser.ToString();

            var notes = await _dbContext.Notes.Where(x => x.UserId == userId).ToListAsync();
            var notesDetails = new List<NoteDetails>();
            foreach (var note in notes)
            {
                var images = await _dbContext.NoteImages.Where(x => x.NoteId == note.NoteId).ToListAsync();
                var score = await _dbContext.Ratings.Where(x => x.NoteId == note.NoteId).SumAsync(x => x.Score);
                var noteTags = await _dbContext.TagsForNotes.Where(x => x.NoteId == note.NoteId).Select(x => x.Tag).ToListAsync();
                notesDetails.Add(new NoteDetails(note, images, user, profileImage, score, noteTags));
            }
        }

        public async Task<IEnumerable<UserRatings>> GetAllUserRatings()
        {
            throw new NotImplementedException();
        }
    }
}
