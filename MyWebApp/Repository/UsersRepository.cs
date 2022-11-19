using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.TableModels;
using MyWebApp.ViewModels;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace MyWebApp.Repository
{
    public sealed class UsersRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly PicturesLoader _picturesLoader;
        private readonly NotesRepository _notesRepository;

        public UsersRepository(ApplicationDbContext dbContext,
            PicturesLoader picturesLoader,
            NotesRepository notesRepository)
        {
            _dbContext = dbContext;
            _picturesLoader = picturesLoader;
            _notesRepository = notesRepository;
        }

        public async Task<UserModel?> GetUser(string userId)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<UserModel?> GetUserNoTracking(string userId)
        {
            return await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<UserImageModel> GetUsersCurrentProfilePicture(UserModel? user)
        {
            if (user == null)
            {
                return _picturesLoader.GetDefaultProfileImage();
            }

            var profilePicture = await _dbContext.ProfileImages.AsNoTracking().OrderBy(x => x.UploadTime).LastOrDefaultAsync(x => x.UserId == user.Id);
            return profilePicture ?? _picturesLoader.GetDefaultProfileImage();
        }

        public async Task<UserModel?> GetNoteAuthor(NoteModel? note)
        {
            if (note == null)
            {
                return null;
            }

            return await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == note.UserId);
        }

        public async Task<IEnumerable<RatingModel>> GetUserRatings(UserModel? user)
        {
            if (user == null)
            {
                return Enumerable.Empty<RatingModel>();
            }

            return await _dbContext.Ratings.AsNoTracking().Where(x => x.UserId == user.Id).ToListAsync();
        }

        public async Task<UserDetailsViewModel> GetUserDetails(string userId)
        {
            var user = await GetUserNoTracking(userId);

            return new UserDetailsViewModel()
            {
                User = await GetUserNoTracking(userId),
                ProfilePicture = await GetUsersCurrentProfilePicture(user)
            };
        }

        public async Task<UserNotesViewModel> GetUserNotes(string userId)
        {
            var user = await GetUserNoTracking(userId);
            var notes = await _notesRepository.GetUsersNotes(user);
            var notesDetails = new List<NoteDetailsViewModel>();
            foreach (var note in notes)
            {
                notesDetails.Add(await _notesRepository.GetNoteDetails(note.NoteId));
            }

            return new UserNotesViewModel()
            {
                User = user,
                ProfilePicture = await GetUsersCurrentProfilePicture(user),
                Notes = notesDetails
            };
        }

        public async Task<UserRatingsViewModel> GetUserRatings(string userId)
        {
            var user = await GetUserNoTracking(userId);
            var ratings = await GetUserRatings(user);

            var noteRatings = new List<NoteRatingViewModel>();
            foreach (var rating in ratings)
            {
                var note = await _notesRepository.GetNoteNoTracking(rating.NoteId);
                if (note == null)
                {
                    continue;
                }

                noteRatings.Add(new NoteRatingViewModel()
                {
                    Rating = rating,
                    NoteDetails = await _notesRepository.GetNoteDetails(note.NoteId)
                });
            }

            return new UserRatingsViewModel()
            {
                User = user,
                ProfilePicture = await GetUsersCurrentProfilePicture(user),
                UsersRatingsOfNotes = noteRatings
            };
        }
    }
}
