using Microsoft.AspNetCore.Mvc;
using MyWebApp.Controllers;
using MyWebApp.Repository;
using MyWebApp.Repository.Interfaces;
using MyWebApp.ViewModels;

namespace MyWebApp.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly IUsersRepository _usersRepository;
        private readonly UsersController _usersController;

        public UsersControllerTests()
        {
            _usersRepository = A.Fake<IUsersRepository>();
            _usersController = new UsersController(_usersRepository);
        }

        [Fact]
        public async Task UsersController_Index_ReturnsSuccess()
        {
            var users = A.Fake<IEnumerable<UserSummaryViewModel>>();
            var count = 0;
            var page = 1;
            var pageSize = 10;
            var offset = (page - 1) * pageSize;
            A.CallTo(() => _usersRepository.GetUsers(offset, pageSize)).Returns(users);
            A.CallTo(() => _usersRepository.GetCount()).Returns(count);

            var result = await _usersController.Index(page, pageSize);

            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task UsersController_Details_ReturnsSuccess()
        {
            var userId = "1";
            var userDetails = A.Fake<UserDetailsViewModel>();
            A.CallTo(() => _usersRepository.GetUserDetails(userId)).Returns(userDetails);

            var result = await _usersController.Details(userId);

            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task UsersController_Notes_ReturnsSuccess()
        {
            var userId = "1";
            var userNotes = A.Fake<UserNotesViewModel>();
            var count = 0;
            var page = 1;
            var pageSize = 6;
            var offset = (page - 1) * pageSize;
            A.CallTo(() => _usersRepository.GetUserNotes(userId, offset, pageSize)).Returns(userNotes);
            A.CallTo(() => _usersRepository.GetCountOfUserNotes(userId)).Returns(count);

            var result = await _usersController.Notes(userId, page, pageSize);

            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();
        }
    }
}
