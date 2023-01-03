using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.PicturesModule;
using MyWebApp.ViewModels;

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

        public async Task<IEnumerable<NoteModel>> GetNotesOfUser(string userId, int offset, int size)
        {
            var user = await GetUserNoTracking(userId);
            if (user == null)
            {
                return Enumerable.Empty<NoteModel>();
            }

            return await _dbContext.Notes.AsNoTracking().Where(x => x.UserId == user.Id).Skip(offset).Take(size).ToListAsync();
        }

        public async Task<int> GetCountOfUserNotes(string userId)
        {
            return await _dbContext.Notes.CountAsync(x => x.UserId == userId);
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

        public async Task<UserNotesViewModel?> GetUserNotes(string userId, int offset, int size)
        {
            var user = await GetUserNoTracking(userId);
            if (user == null)
            {
                return null;
            }

            var notes = await GetNotesOfUser(user.Id, offset, size);
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
    }
}
