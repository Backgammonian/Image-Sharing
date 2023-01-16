using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Data.Interfaces;
using MyWebApp.Models;
using MyWebApp.PicturesModule.Interfaces;
using MyWebApp.Repository;
using MyWebApp.ViewModels;

namespace MyWebApp.Tests.Repositories
{
    public class NotesRepositoryTests
    {
        private readonly IRandomGenerator _randomGenerator;
        private readonly IPicturesLoader _picturesLoader;
        private readonly PlaceholderDatabaseGenerator _placeholderDatabaseGenerator;
        private readonly PlaceholderImageGenerator _placeholderImageGenerator;

        public NotesRepositoryTests()
        {
            _randomGenerator = A.Fake<IRandomGenerator>();
            _picturesLoader = A.Fake<IPicturesLoader>();
            _placeholderDatabaseGenerator = new PlaceholderDatabaseGenerator();
            _placeholderImageGenerator = new PlaceholderImageGenerator();
        }

        private async Task<ApplicationDbContext> GetDatabase()
        {
            return await _placeholderDatabaseGenerator.GetDatabase();
        }

        private async Task<NotesRepository> GetRepository()
        {
            return new NotesRepository(_randomGenerator,
                _picturesLoader,
                await _placeholderDatabaseGenerator.GetDatabase());
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

            result.Should().BeGreaterThan(0);
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

        [Fact]
        public async Task NotesRepository_Create_ReturnsSuccess()
        {
            var database = await GetDatabase();
            var oldNotesCount = await database.Notes.CountAsync();
            var oldNoteImagesCount = await database.NoteImages.CountAsync();
            var oldNoteThreadsCount = await database.NoteThreads.CountAsync();

            var notesRepository = new NotesRepository(_randomGenerator, _picturesLoader, database);
            var user = await database.Users.AsNoTracking().FirstOrDefaultAsync();
            var threads = await database.Threads.AsNoTracking().ToListAsync();
            var thread = threads[0];

            var createNoteVM = new CreateNoteViewModel()
            {
                Title = "New title",
                Description = "New description",
                SelectedThread = thread.Thread,
                Images = new List<IFormFile>()
                {
                    _placeholderImageGenerator.GetImage()
                }
            };

            var noteImage = A.Fake<NoteImageModel>();
            var image = A.Fake<IFormFile>();
            var note = A.Fake<NoteModel>();
            var newNoteId = "1234";
            A.CallTo(() => _randomGenerator.GetRandomId()).Returns(newNoteId);
            A.CallTo(() => _picturesLoader.LoadNoteImage(image, note)).Returns(noteImage);

            var result = await notesRepository.Create(user, createNoteVM);
            var newNotesCount = await database.Notes.CountAsync();
            var newNoteImagesCount = await database.NoteImages.CountAsync();
            var newNoteThreadsCount = await database.NoteThreads.CountAsync();
            var newNote = await database.Notes.AsNoTracking().FirstOrDefaultAsync(x => x.NoteId == newNoteId);
            var newNoteThread = await database.NoteThreads.AsNoTracking().FirstOrDefaultAsync(x => x.NoteId == newNote.NoteId);

            result.Should().NotBeNull();
            result.Should().BeOfType<string>();
            result.Should().NotBeEmpty();
            newNotesCount.Should().Be(oldNotesCount + 1);
            newNoteImagesCount.Should().Be(oldNoteImagesCount + 1);
            newNoteThreadsCount.Should().Be(oldNoteThreadsCount + 1);
            newNote.Title.Should().Be(createNoteVM.Title);
            newNote.Description.Should().Be(createNoteVM.Description);
            newNoteThread.Thread.Should().Be(createNoteVM.SelectedThread);
        }

        [Fact]
        public async Task NotesRepository_Update_ReturnsSuccess()
        {
            var database = await GetDatabase();
            var notesRepository = new NotesRepository(_randomGenerator, _picturesLoader, database);
            var editableNoteId = "0";

            var editNoteVM = new EditNoteViewModel()
            {
                Title = "NEWEST title",
                Description = "NEWEST description",
                NoteId = editableNoteId,
            };

            var noteImage = A.Fake<NoteImageModel>();
            var image = A.Fake<IFormFile>();
            var note = A.Fake<NoteModel>();
            A.CallTo(() => _randomGenerator.GetRandomId()).Returns("4321");
            A.CallTo(() => _picturesLoader.LoadNoteImage(image, note)).Returns(noteImage);

            var result = await notesRepository.Update(editableNoteId, editNoteVM);
            var editedNote = await database.Notes.AsNoTracking().FirstOrDefaultAsync(x => x.NoteId == editableNoteId);

            result.Should().BeTrue();
            editedNote.Title.Should().Be(editNoteVM.Title);
            editedNote.Description.Should().Be(editNoteVM.Description);
        }

        [Fact]
        public async Task NotesRepository_Delete_ReturnsSuccess()
        {
            var database = await GetDatabase();
            var oldNotesCount = await database.Notes.CountAsync();
            var notesRepository = new NotesRepository(_randomGenerator, _picturesLoader, database);
            var deletableNoteId = "0";

            var deleteNoteVM = new DeleteNoteViewModel()
            {
                NoteId = deletableNoteId,
            };

            A.CallTo(() => _randomGenerator.GetRandomId()).Returns("4321");

            var result = await notesRepository.Delete(deleteNoteVM);
            var newNotesCount = await database.Notes.CountAsync();
            var previousNotesCount = await database.PreviousNotes.CountAsync();

            result.Should().BeTrue();
            previousNotesCount.Should().Be(1);
            newNotesCount.Should().Be(oldNotesCount - 1);
        }
    }
}
