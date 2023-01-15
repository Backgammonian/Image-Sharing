using Microsoft.AspNetCore.Identity;
using MyWebApp.ViewModels;
using MyWebApp.Models;
using MyWebApp.Data;
using MyWebApp.PicturesModule.Interfaces;
using MyWebApp.Repository.Interfaces;

namespace MyWebApp.Repository
{
    public sealed class DashboardRepository : IDashboardRepository
    {
        private readonly IPicturesLoader _picturesLoader;
        private readonly INotesRepository _notesRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<UserModel> _userManager;

        public DashboardRepository(IPicturesLoader picturesLoader,
            INotesRepository notesRepository,
            IUsersRepository usersRepository,
            ApplicationDbContext dbContext,
            UserManager<UserModel> userManager)
        {
            _notesRepository = notesRepository;
            _usersRepository = usersRepository;
            _picturesLoader = picturesLoader;
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<int> GetNotesCount(UserModel? user)
        {
            if (user == null)
            {
                return 0;
            }

            return await _usersRepository.GetCountOfUserNotes(user.Id);
        }

        public async Task<DashboardViewModel> GetDashboard(UserModel user, int offset, int size)
        {
            var userNotes = await _usersRepository.GetNotesOfUser(user.Id, offset, size);
            var notesDetails = new List<NoteDetailsViewModel>();
            foreach (var userNote in userNotes)
            {
                var noteDetail = await _notesRepository.GetNoteDetails(userNote.NoteId);
                notesDetails.Add(noteDetail);
            }

            return new DashboardViewModel()
            {
                UserNotes = notesDetails,
                User = user,
                ProfilePicture = await _usersRepository.GetUsersCurrentProfilePicture(user)
            };
        }

        public async Task<bool> Update(UserModel user, EditUserProfileViewModel editUserProfileVM)
        {           
            var newProfileImage = editUserProfileVM.NewProfilePicture;
            if (newProfileImage != null)
            {
                var userImage = await _picturesLoader.LoadProfileImage(newProfileImage, user);
                await _dbContext.ProfileImages.AddAsync(userImage);
            }

            user.Status = editUserProfileVM.Status;
            user.UserName = editUserProfileVM.UserName;
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
