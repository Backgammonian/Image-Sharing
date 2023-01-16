using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.PicturesModule.Interfaces;
using MyWebApp.Repository;
using MyWebApp.Repository.Interfaces;
using MyWebApp.ViewModels;
using System.Drawing;

namespace MyWebApp.Tests.Repositories
{
    public class DashboardRepositoryTests
    {
        private readonly IPicturesLoader _picturesLoader;
        private readonly INotesRepository _notesRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly UserManager<UserModel> _userManager;
        private readonly PlaceholderDatabaseGenerator _placeholderDatabaseGenerator;
        private readonly PlaceholderImageGenerator _placeholderImageGenerator;

        public DashboardRepositoryTests()
        {
            _picturesLoader = A.Fake<IPicturesLoader>();
            _notesRepository = A.Fake<INotesRepository>();
            _usersRepository = A.Fake<IUsersRepository>();
            _userManager = A.Fake<UserManager<UserModel>>();
            _placeholderDatabaseGenerator = new PlaceholderDatabaseGenerator();
            _placeholderImageGenerator = new PlaceholderImageGenerator();
        }

        private async Task<ApplicationDbContext> GetDatabase()
        {
            return await _placeholderDatabaseGenerator.GetDatabase();
        }

        private async Task<DashboardRepository> GetRepository()
        {
            return new DashboardRepository(_picturesLoader,
                _notesRepository,
                _usersRepository,
                await _placeholderDatabaseGenerator.GetDatabase(),
                _userManager);
        }

        [Fact]
        public async Task DashboardRepository_GetDashboard_ReturnsSuccess()
        {
            var dashboardRepository = await GetRepository();
            var page = 1;
            var pageSize = 6;
            var offset = (page - 1) * pageSize;
            var user = A.Fake<UserModel>();
            var userNotes = A.Fake<IEnumerable<NoteModel>>();
            A.CallTo(() => _usersRepository.GetNotesOfUser(user.Id, offset, pageSize)).Returns(userNotes);
            var userNoteId = "1234";
            var noteDetails = A.Fake<NoteDetailsViewModel>();
            A.CallTo(() => _notesRepository.GetNoteDetails(userNoteId)).Returns(noteDetails);
            var profilePicture = A.Fake<UserImageModel>();
            A.CallTo(() => _usersRepository.GetUsersCurrentProfilePicture(user)).Returns(profilePicture);

            var result = await dashboardRepository.GetDashboard(user, offset, pageSize);

            result.Should().NotBeNull();
            result.Should().BeOfType<DashboardViewModel>();
        }

        [Fact]
        public async Task DashboardRepository_Update_ReturnsSuccess()
        {
            var database = await GetDatabase();
            var dashboardRepository = new DashboardRepository(_picturesLoader,
                _notesRepository,
                _usersRepository,
                database,
                _userManager);
            var user = await database.Users.FirstOrDefaultAsync();
            var oldUserName = user.UserName;
            var oldUserStatus = user.Status;
            var editUserProfileVM = new EditUserProfileViewModel()
            {
                UserId = user.Id,
                UserName = "NewUserName",
                Status = "NewUserStatus",
                NewProfilePicture = _placeholderImageGenerator.GetImage()
            };
            var image = _placeholderImageGenerator.GetImage();
            var profilePicture = A.Fake<UserImageModel>();
            A.CallTo(() => _picturesLoader.LoadProfileImage(image, user)).Returns(profilePicture);

            var result = await dashboardRepository.Update(user, editUserProfileVM);
            var updatedUser = await database.Users.AsNoTracking().FirstOrDefaultAsync();
            var newUserName = updatedUser.UserName;
            var newUserStatus = updatedUser.Status;

            result.Should().BeTrue();
            oldUserName.Should().NotBe(newUserName);
            oldUserStatus.Should().NotBe(newUserStatus);
            newUserName.Should().Be(editUserProfileVM.UserName);
            newUserStatus.Should().Be(editUserProfileVM.Status);
        }
    }
}
