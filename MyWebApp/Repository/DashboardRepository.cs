using MyWebApp.ViewModels;
using MyWebApp.Extensions;
using MyWebApp.Models;
using MyWebApp.Data;

namespace MyWebApp.Repository
{
    public sealed class DashboardRepository
    {

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly NotesRepository _notesRepository;
        private readonly UsersRepository _usersRepository;
        private readonly PicturesLoader _picturesLoader;
        private readonly ApplicationDbContext _dbContext;

        public DashboardRepository(IHttpContextAccessor contextAccessor,
            NotesRepository notesRepository,
            UsersRepository usersRepository,
            PicturesLoader picturesLoader,
            ApplicationDbContext dbContext)
        {
            _contextAccessor = contextAccessor;
            _notesRepository = notesRepository;
            _usersRepository = usersRepository;
            _picturesLoader = picturesLoader;
            _dbContext = dbContext;
        }

        public async Task<DashboardViewModel?> GetDashboard()
        {
            var currentUserId = _contextAccessor.HttpContext?.User.GetUserId();
            if (currentUserId == null ||
                currentUserId.IsEmpty())
            {
                return null;
            }

            var userNotes = await _usersRepository.GetNotesOfUser(currentUserId);
            var notesDetails = new List<NoteDetailsViewModel>();
            foreach (var userNote in userNotes)
            {
                var noteDetail = await _notesRepository.GetNoteDetails(userNote.NoteId);
                notesDetails.Add(noteDetail);
            }

            return new DashboardViewModel()
            {
                UserNotes = notesDetails,
                UserRatings = await _usersRepository.GetUserRatings(currentUserId)
            };
        }

        public async Task<UserModel?> GetCurrentUserNoTracking()
        {
            var currentUserId = _contextAccessor.HttpContext?.User.GetUserId();
            if (currentUserId == null ||
                currentUserId.IsEmpty())
            {
                return null;
            }

            return await _usersRepository.GetUserNoTracking(currentUserId);
        }

        public async Task<UserModel?> GetCurrentUser()
        {
            var currentUserId = _contextAccessor.HttpContext?.User.GetUserId();
            if (currentUserId == null ||
                currentUserId.IsEmpty())
            {
                return null;
            }

            return await _usersRepository.GetUser(currentUserId);
        }

        public async Task<bool> Update(UserModel user, EditUserProfileViewModel editUserProfileVM)
        {
            if (user == null)
            {
                return false;
            }

            user.Status = editUserProfileVM.Status;

            var newProfileImage = editUserProfileVM.ProfileImage;
            if (newProfileImage != null)
            {
                var userImage = await _picturesLoader.LoadProfileImage(newProfileImage, user);
                await _dbContext.ProfileImages.AddAsync(userImage);
            }

            _dbContext.Users.Update(user);

            return await Save();
        }

        public async Task<bool> Save()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
