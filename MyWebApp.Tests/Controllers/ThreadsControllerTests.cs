using Microsoft.AspNetCore.Mvc;
using MyWebApp.Controllers;
using MyWebApp.Models;
using MyWebApp.Repository.Interfaces;
using MyWebApp.ViewModels;

namespace MyWebApp.Tests.Controllers
{
    public class ThreadsControllerTests
    {
        private readonly IThreadsRepository _threadsRepository;
        private readonly ThreadsController _threadsController;

        public ThreadsControllerTests()
        {
            _threadsRepository = A.Fake<IThreadsRepository>();
            _threadsController = new ThreadsController(_threadsRepository);
        }

        [Fact]
        public async Task ThreadsController_Index_ReturnsSuccess()
        {
            var availableThreads = A.Fake<IEnumerable<ThreadModel>>();
            A.CallTo(() => _threadsRepository.GetAllThreads()).Returns(availableThreads);

            var result = await _threadsController.Index();

            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task ThreadsController_GetByThread_ReturnsSuccess()
        {
            var notesFromThread = A.Fake<NotesFromThreadViewModel>();
            var thread = "funny";
            var count = 0;
            var page = 1;
            var pageSize = 6;
            var offset = (page - 1) * pageSize;
            A.CallTo(() => _threadsRepository.GetByThread(thread, offset, pageSize)).Returns(notesFromThread);
            A.CallTo(() => _threadsRepository.GetCountOfNotesFromThread(thread)).Returns(count);

            var result = await _threadsController.GetByThread(thread, page, pageSize);

            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();
        }
    }
}
