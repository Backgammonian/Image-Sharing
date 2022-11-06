using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.Repository.Interfaces;

namespace MyWebApp.Repository
{
    public sealed class UsersRepository : IUsersRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly PicturesLoader _picturesLoader;

        public UsersRepository(ApplicationDbContext dbContext, PicturesLoader picturesLoader)
        {
            _dbContext = dbContext;
            _picturesLoader = picturesLoader;
        }

        public async Task<UserDetails?> GetUserInfo(string userId)
        {
            if (userId == null ||
                userId == string.Empty)
            {
                return null;
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null)
            {
                return null;
            }

            var profileImage = await _dbContext.ProfileImages.LastOrDefaultAsync(x => x.UserId == user.UserId);
            profileImage ??= _picturesLoader.DefaultProfileImage;

            return new UserDetails(user, profileImage);
        }

        public async Task<UserNotes?> GetUserNotes(string userId)
        {
            if (userId == null ||
                userId == string.Empty)
            {
                return null;
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null)
            {
                return null;
            }

            var profileImage = await _dbContext.ProfileImages.LastOrDefaultAsync(x => x.UserId == user.UserId);
            profileImage ??= _picturesLoader.DefaultProfileImage;

            var notes = await _dbContext.Notes.Where(x => x.UserId == userId).ToListAsync();
            var notesDetails = new List<NoteDetails>();
            foreach (var note in notes)
            {
                var images = await _dbContext.NoteImages.Where(x => x.NoteId == note.NoteId).ToListAsync();
                var score = await _dbContext.Ratings.Where(x => x.NoteId == note.NoteId).SumAsync(x => x.Score);
                var noteTags = await _dbContext.TagsForNotes.Where(x => x.NoteId == note.NoteId).Select(x => x.Tag).ToListAsync();
                notesDetails.Add(new NoteDetails(note, images, user, profileImage, score, noteTags));
            }

            return new UserNotes(user, profileImage, notesDetails);
        }

        public async Task<UserRatings?> GetUserRatings(string userId)
        {
            if (userId == null ||
                userId == string.Empty)
            {
                return null;
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null)
            {
                return null;
            }

            var profileImage = await _dbContext.ProfileImages.LastOrDefaultAsync(x => x.UserId == user.UserId);
            profileImage ??= _picturesLoader.DefaultProfileImage;

            var ratings = await _dbContext.Ratings.Where(x => x.UserId == userId).ToListAsync();
            var noteRatings = new List<NoteRating>();
            foreach (var rating in ratings)
            {
                var note = await _dbContext.Notes.FirstOrDefaultAsync(x => x.NoteId == rating.NoteId);
                if (note == null)
                {
                    continue;
                }

                noteRatings.Add(new NoteRating(note, rating));
            }

            return new UserRatings(user, profileImage, noteRatings);
        }
    }
}
