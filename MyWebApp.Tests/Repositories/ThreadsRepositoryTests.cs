using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.Repository;
using MyWebApp.Repository.Interfaces;
using MyWebApp.ViewModels;

namespace MyWebApp.Tests.Repositories
{
    public class ThreadsRepositoryTests
    {
        private readonly INotesRepository _notesRepository;
        private readonly PlaceholderDatabaseGenerator _placeholderDatabaseGenerator;

        public ThreadsRepositoryTests()
        {
            _notesRepository = A.Fake<INotesRepository>();
            _placeholderDatabaseGenerator = new PlaceholderDatabaseGenerator();
        }

        private async Task<ApplicationDbContext> GetDatabase()
        {
            return await _placeholderDatabaseGenerator.GetDatabase();
        }

        private async Task<ThreadsRepository> GetRepository()
        {
            return new ThreadsRepository(_notesRepository,
                await _placeholderDatabaseGenerator.GetDatabase());
        }

        [Fact]
        public async Task ThreadsRepository_GetNotesFromThread_ReturnsSuccess()
        {
            var threadsRepository = await GetRepository();
            var page = 1;
            var pageSize = 6;
            var offset = (page - 1) * pageSize;
            var thread = "funny";

            var result = await threadsRepository.GetNotesFromThread(thread, offset, pageSize);

            result.Should().NotBeNull();
            result.Should().BeOfType<List<NoteThreadModel>>();
            result.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task ThreadsRepository_GetCountOfNotesFromThread_ReturnsSuccess()
        {
            var threadsRepository = await GetRepository();
            var thread = "funny";

            var result = await threadsRepository.GetCountOfNotesFromThread(thread);

            result.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task ThreadsRepository_GetAllThreads_ReturnsSuccess()
        {
            var threadsRepository = await GetRepository();

            var result = await threadsRepository.GetAllThreads();

            result.Should().NotBeNull();
            result.Should().BeOfType<List<ThreadModel>>();
            result.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task ThreadsRepository_GetByThread_ReturnsSuccess()
        {
            var threadsRepository = await GetRepository();
            var page = 1;
            var pageSize = 6;
            var offset = (page - 1) * pageSize;
            var thread = "funny";
            var noteId = "1234";
            var note = A.Fake<NoteModel>();
            var noteDetails = A.Fake<NoteDetailsViewModel>();
            A.CallTo(() => _notesRepository.GetNoteNoTracking(noteId)).Returns(note);
            A.CallTo(() => _notesRepository.GetNoteDetails(note.NoteId)).Returns(noteDetails);

            var result = await threadsRepository.GetByThread(thread, offset, pageSize);

            result.Should().NotBeNull();
            result.Should().BeOfType<NotesFromThreadViewModel>();
        }

        [Fact]
        public async Task ThreadsRepository_Create_ReturnsSuccess()
        {
            var database = await GetDatabase();
            var threadsRepository = new ThreadsRepository(_notesRepository, database);
            var oldThreadsCount = await database.Threads.CountAsync();
            var createThreadVM = new CreateThreadViewModel()
            {
                NewThreadName = "nEwThReAd"
            };

            var result = await threadsRepository.Create(createThreadVM);
            var newThreadsCount = await database.Threads.CountAsync();

            result.Should().BeTrue();
            newThreadsCount.Should().Be(oldThreadsCount + 1);
        }

        [Fact]
        public async Task ThreadsRepository_Delete_ReturnsSuccess()
        {
            var database = await GetDatabase();
            var threadsRepository = new ThreadsRepository(_notesRepository, database);
            var oldThreadsCount = await database.Threads.CountAsync();
            var oldNoteThreadsCount = await database.NoteThreads.CountAsync();
            var deleteThreadVM = new DeleteThreadViewModel()
            {
                SelectedThreadName = "photos"
            };

            var result = await threadsRepository.Delete(deleteThreadVM);
            var newThreadsCount = await database.Threads.CountAsync();
            var newNoteThreadsCount = await database.NoteThreads.CountAsync();

            result.Should().BeTrue();
            newThreadsCount.Should().Be(oldThreadsCount - 1);
            newNoteThreadsCount.Should().BeLessThanOrEqualTo(oldNoteThreadsCount);
        }
    }
}
