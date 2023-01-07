using MyWebApp.ViewModels;
using MyWebApp.Models;
using MyWebApp.Data;
using MyWebApp.PicturesModule;
using Microsoft.AspNetCore.Identity;

namespace MyWebApp.Repository
{
    public sealed class DashboardRepository
    {
        private readonly CredentialsRepository _credentialsRepository;
        private readonly NotesRepository _notesRepository;
        private readonly UsersRepository _usersRepository;
        private readonly PicturesLoader _picturesLoader;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<UserModel> _userManager;

        public DashboardRepository(CredentialsRepository credentialsRepository,
            NotesRepository notesRepository,
            UsersRepository usersRepository,
            PicturesLoader picturesLoader,
            ApplicationDbContext dbContext,
            UserManager<UserModel> userManager)
        {
            _credentialsRepository = credentialsRepository;
            _notesRepository = notesRepository;
            _usersRepository = usersRepository;
            _picturesLoader = picturesLoader;
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<UserImageModel> GetCurrentUserProfilePicture()
        {
            var credentials = await _credentialsRepository.GetLoggedInUser();
            var currentUser = credentials.User;

            return await _usersRepository.GetUsersCurrentProfilePicture(currentUser);
        }

        public async Task<int> GetNotesCount()
        {
            var credentials = await _credentialsRepository.GetLoggedInUser();
            var currentUser = credentials.User;
            if (currentUser == null)
            {
                return 0;
            }

            return await _usersRepository.GetCountOfUserNotes(currentUser.Id);
        }

        public async Task<DashboardViewModel?> GetDashboard(int offset, int size)
        {
            var credentials = await _credentialsRepository.GetLoggedInUser();
            var currentUser = credentials.User;
            if (currentUser == null)
            {
                return null;
            }

            var userNotes = await _usersRepository.GetNotesOfUser(currentUser.Id, offset, size);
            var notesDetails = new List<NoteDetailsViewModel>();
            foreach (var userNote in userNotes)
            {
                var noteDetail = await _notesRepository.GetNoteDetails(userNote.NoteId);
                notesDetails.Add(noteDetail);
            }

            return new DashboardViewModel()
            {
                UserNotes = notesDetails,
                User = currentUser,
                ProfilePicture = await _usersRepository.GetUsersCurrentProfilePicture(currentUser)
            };
        }

        public async Task<bool> Update(UserModel user, EditUserProfileViewModel editUserProfileVM)
        {
            if (user == null)
            {
                return false;
            }
            
            var newProfileImage = editUserProfileVM.NewProfilePicture;
            if (newProfileImage != null)
            {
                var userImage = await _picturesLoader.LoadProfileImage(newProfileImage, user);
                await _dbContext.ProfileImages.AddAsync(userImage);
            }

            user.Status = editUserProfileVM.Status;
            _dbContext.Users.Update(user);

            return await Save();
        }

        public async Task<bool> UpdatePassword(string userId, EditPasswordViewModel editPasswordVM)
        {
            if (editPasswordVM.NewPassword != editPasswordVM.ConfirmNewPassword)
            {
                return false;
            }

            var user = await _userManager.FindByIdAsync(userId);
            var passwordCheck = await _userManager.CheckPasswordAsync(user, editPasswordVM.OldPassword);
            if (!passwordCheck)
            {
                return false;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, editPasswordVM.NewPassword);

            return result.Succeeded;
        }

        public async Task<bool> Save()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
