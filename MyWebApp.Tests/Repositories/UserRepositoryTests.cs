using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.PicturesModule.Interfaces;
using MyWebApp.Repository;
using MyWebApp.Repository.Interfaces;
using MyWebApp.ViewModels;

namespace MyWebApp.Tests.Repositories
{
    public class UserRepositoryTests
    {
        private readonly IPicturesLoader _picturesLoader;
        private readonly INotesRepository _notesRepository;
        private readonly PlaceholderDatabaseGenerator _placeholderDatabaseGenerator;

        public UserRepositoryTests()
        {
            _picturesLoader = A.Fake<IPicturesLoader>();
            _notesRepository = A.Fake<INotesRepository>();
            _placeholderDatabaseGenerator = new PlaceholderDatabaseGenerator();
        }

        private async Task<ApplicationDbContext> GetDatabase()
        {
            return await _placeholderDatabaseGenerator.GetDatabase();
        }

        private async Task<UsersRepository> GetRepository()
        {
            return new UsersRepository(_picturesLoader,
                _notesRepository,
                await _placeholderDatabaseGenerator.GetDatabase());
        }

        [Fact]
        public async Task UsersRepository_GetUsersSlice_ReturnsSuccess()
        {
            var usersRepository = await GetRepository();
            var page = 1;
            var pageSize = 10;
            var offset = (page - 1) * pageSize;

            var result = await usersRepository.GetUsersSlice(offset, pageSize);

            result.Should().NotBeNull();
            result.Should().BeOfType<List<UserModel>>();
            result.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task UsersRepository_GetUsers_ReturnsSuccess()
        {
            var usersRepository = await GetRepository();
            var page = 1;
            var pageSize = 10;
            var offset = (page - 1) * pageSize;

            var result = await usersRepository.GetUsers(offset, pageSize);

            result.Should().NotBeNull();
            result.Should().BeOfType<List<UserSummaryViewModel>>();
            result.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task UsersRepository_GetCount_ReturnsSuccess()
        {
            var usersRepository = await GetRepository();

            var result = await usersRepository.GetCount();

            result.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task UsersRepository_GetUser_ReturnsSuccess()
        {
            var usersRepository = await GetRepository();
            var userId = "0";

            var result = await usersRepository.GetUser(userId);

            result.Should().NotBeNull();
            result.Should().BeOfType<UserModel>();
        }

        [Fact]
        public async Task UsersRepository_GetUserNoTracking_ReturnsSuccess()
        {
            var usersRepository = await GetRepository();
            var userId = "0";

            var result = await usersRepository.GetUserNoTracking(userId);

            result.Should().NotBeNull();
            result.Should().BeOfType<UserModel>();
        }

        [Fact]
        public async Task UsersRepository_GetUsersCurrentProfilePictureById_ReturnsSuccess()
        {
            var usersRepository = await GetRepository();
            var userId = "0";
            var defaultProfilePicture = A.Fake<UserImageModel>();
            A.CallTo(() => _picturesLoader.GetDefaultProfileImage()).Returns(defaultProfilePicture);

            var result = await usersRepository.GetUsersCurrentProfilePicture(userId);

            result.Should().NotBeNull();
            result.Should().BeOfType<UserImageModel>();
        }

        [Fact]
        public async Task UsersRepository_GetUsersCurrentProfilePictureByUserModel_ReturnsSuccess()
        {
            var usersRepository = await GetRepository();
            var userId = "0";
            var defaultProfilePicture = A.Fake<UserImageModel>();
            A.CallTo(() => _picturesLoader.GetDefaultProfileImage()).Returns(defaultProfilePicture);

            var user = await usersRepository.GetUserNoTracking(userId);
            var result = await usersRepository.GetUsersCurrentProfilePicture(user);

            user.Should().NotBeNull();
            result.Should().NotBeNull();
            result.Should().BeOfType<UserImageModel>();
        }

        [Fact]
        public async Task UsersRepository_GetNotesOfUser_ReturnsSuccess()
        {
            var usersRepository = await GetRepository();
            var userId = "0";
            var page = 1;
            var pageSize = 10;
            var offset = (page - 1) * pageSize;

            var result = await usersRepository.GetNotesOfUser(userId, offset, pageSize);

            result.Should().NotBeNull();
            result.Should().BeOfType<List<NoteModel>>();
            result.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task UsersRepository_GetCountOfUserNotes_ReturnsSuccess()
        {
            var usersRepository = await GetRepository();
            var userId = "0";

            var result = await usersRepository.GetCountOfUserNotes(userId);

            result.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task UsersRepository_GetUserDetails_ReturnsSuccess()
        {
            var usersRepository = await GetRepository();
            var userId = "0";

            var result = await usersRepository.GetUserDetails(userId);

            result.Should().NotBeNull();
            result.Should().BeOfType<UserDetailsViewModel>();
        }

        [Fact]
        public async Task UsersRepository_GetDetailedUserNotes_ReturnsSuccess()
        {
            var usersRepository = await GetRepository();
            var userId = "0";
            var page = 1;
            var pageSize = 6;
            var offset = (page - 1) * pageSize;
            var noteDetailsVM = A.Fake<NoteDetailsViewModel>();
            var noteId = "0";
            A.CallTo(() => _notesRepository.GetNoteDetails(noteId)).Returns(noteDetailsVM);

            var result = await usersRepository.GetDetailedUserNotes(userId, offset, pageSize);

            result.Should().NotBeNull();
            result.Should().BeOfType<UserNotesViewModel>();
        }
    }
}
