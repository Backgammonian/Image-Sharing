using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Data.Interfaces;
using MyWebApp.Models;
using MyWebApp.PicturesModule.Interfaces;
using MyWebApp.Repository;
using MyWebApp.Repository.Interfaces;

namespace MyWebApp.Tests.Repositories
{
    public class NotesRepositoryTests
    {
        private readonly IRandomGenerator _randomGenerator;
        private readonly IPicturesLoader _picturesLoader;
        private readonly ICredentialsRepository _credentialsRepository;

        public NotesRepositoryTests()
        {
            _randomGenerator = A.Fake<IRandomGenerator>();
            _picturesLoader = A.Fake<IPicturesLoader>();
            _credentialsRepository = A.Fake<ICredentialsRepository>();
        }

        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var dbContext = new ApplicationDbContext(options);
            dbContext.Database.EnsureCreated();

            if (!dbContext.Notes.Any() &&
                !dbContext.NoteThreads.Any())
            {
                var notes = new List<NoteModel>();
                var noteThreads = new List<NoteThreadModel>();
                for (int i = 0; i < 10; i++)
                {
                    notes.Add(new NoteModel()
                    {
                        NoteId = (i + 1).ToString(),
                        UserId = i >= 5 ? "1" : "2",
                        Title = "Simple note title",
                        Description = "Sample text of note"
                    });

                    noteThreads.Add(new NoteThreadModel()
                    {
                        Id = (i + 1).ToString(),
                        NoteId = (i + 1).ToString(),
                        Thread = i >= 5 ? "funny" : "photos"
                    });
                }

                await dbContext.Notes.AddRangeAsync(notes);
                await dbContext.NoteThreads.AddRangeAsync(noteThreads);
            }

            if (!dbContext.Threads.Any())
            {
                await dbContext.Threads.AddAsync(new ThreadModel()
                {
                    Thread = "funny"
                });

                await dbContext.Threads.AddAsync(new ThreadModel()
                {
                    Thread = "photos"
                });
            }

            if (!dbContext.Users.Any())
            {
                await dbContext.Users.AddAsync(new UserModel()
                {
                    Id = "1",
                    UserName = "first"
                });

                await dbContext.Users.AddAsync(new UserModel()
                {
                    Id = "2",
                    UserName = "theSecond"
                });
            }

            if (!dbContext.NoteImages.Any())
            {
                await dbContext.NoteImages.AddAsync(new NoteImageModel()
                {
                    ImageId = "1",
                    NoteId = "1",
                    ImageFileName = "image1.jpg",
                    UploadTime = DateTimeOffset.Now.AddDays(-1)
                });

                await dbContext.NoteImages.AddAsync(new NoteImageModel()
                {
                    ImageId = "2",
                    NoteId = "2",
                    ImageFileName = "image2.jpg",
                    UploadTime = DateTimeOffset.Now.AddDays(-2)
                });
            }

            await dbContext.SaveChangesAsync();

            return dbContext;
        }

        private async Task<NotesRepository> GetRepository()
        {
            var dbContext = await GetDbContext();
            return new NotesRepository(_randomGenerator,
                _picturesLoader,
                _credentialsRepository,
                dbContext);
        }

        [Fact]
        public async Task NotesRepository_GetAvailableNoteThreads_ReturnsSuccess()
        {
            var notesRepository = await GetRepository();

            var result = await notesRepository.GetAvailableNoteThreads();

            result.Should().NotBeNull();
            result.Should().BeOfType<List<ThreadModel>>();
        }

        [Fact]
        public async Task NotesRepository_GetNotesSlice_ReturnsSuccess()
        {
            var notesRepository = await GetRepository();
            var page = 1;
            var pageSize = 6;
            var offset = (page - 1) * pageSize;

            var result = await notesRepository.GetNotesSlice(offset, pageSize);

            result.Should().NotBeNull();
            result.Should().BeOfType<List<NoteModel>>();
        }

        [Fact]
        public async Task NotesRepository_GetCount_ReturnsSuccess()
        {
            var notesRepository = await GetRepository();

            var result = await notesRepository.GetCount();

            result.Should().Be(10);
        }

        [Fact]
        public async Task NotesRepository_GetNote_ReturnsSuccess()
        {
            var notesRepository = await GetRepository();
            var noteId = "1";
            var nonExistingNoteId = "42";

            var resultNote = await notesRepository.GetNote(noteId);
            var resultNonExistingNote = await notesRepository.GetNote(nonExistingNoteId);

            resultNote.Should().NotBeNull();
            resultNote.Should().BeOfType<NoteModel>();
            resultNonExistingNote.Should().BeNull();
        }

        [Fact]
        public async Task NotesRepository_GetNoteNoTracking_ReturnsSuccess()
        {
            var notesRepository = await GetRepository();
            var noteId = "1";
            var nonExistingNoteId = "42";

            var resultNote = await notesRepository.GetNoteNoTracking(noteId);
            var resultNonExistingNote = await notesRepository.GetNoteNoTracking(nonExistingNoteId);

            resultNote.Should().NotBeNull();
            resultNote.Should().BeOfType<NoteModel>();
            resultNonExistingNote.Should().BeNull();
        }

        [Fact]
        public async Task NotesRepository_GetNoteAuthor_ReturnsSuccess()
        {
            var notesRepository = await GetRepository();
            var noteId = "1";
            var nonExistingNoteId = "42";

            var note = await notesRepository.GetNote(noteId);
            var nonExistingNote = await notesRepository.GetNote(nonExistingNoteId);
            var result1 = await notesRepository.GetNoteAuthor(note);
            var result2 = await notesRepository.GetNoteAuthor(nonExistingNote);

            result1.Should().NotBeNull();
            result1.Should().BeOfType<UserModel>();
            result2.Should().BeNull();
        }

        [Fact]
        public async Task NotesRepository_GetNoteThread_ReturnsSuccess()
        {
            var notesRepository = await GetRepository();
            var noteId = "1";

            var result = await notesRepository.GetNoteThread(noteId);

            result.Should().NotBeNull();
            result.Should().BeOfType<NoteThreadModel>();
        }

        [Fact]
        public async Task NotesRepository_GetNoteFirstImage_ReturnsSuccess()
        {
            var notesRepository = await GetRepository();
            var noteId = "1";

            var result = await notesRepository.GetNoteFirstImage(noteId);

            result.Should().NotBeNull();
            result.Should().BeOfType<NoteImageModel>();
        }

        [Fact]
        public async Task NotesRepository_GetNoteImages_ReturnsSuccess()
        {
            var notesRepository = await GetRepository();
            var noteId = "1";

            var result = await notesRepository.GetNoteImages(noteId);

            result.Should().NotBeNull();
            result.Should().BeOfType<List<NoteImageModel>>();
        }

        [Fact]
        public async Task NotesRepository_GetNoteImagesNoTracking_ReturnsSuccess()
        {
            var notesRepository = await GetRepository();
            var noteId = "1";

            var result = await notesRepository.GetNoteImagesNoTracking(noteId);

            result.Should().NotBeNull();
            result.Should().BeOfType<List<NoteImageModel>>();
        }
    }
}
