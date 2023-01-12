using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyWebApp.Controllers;
using MyWebApp.Localization.Interfaces;
using MyWebApp.Repository.Interfaces;
using MyWebApp.ViewModels;

namespace MyWebApp.Tests.Controllers
{
    public class NotesControllerTests
    {
        private readonly NotesController _notesController;
        private readonly ILogger<NotesController> _logger;
        private readonly ILanguageService _languageService;
        private readonly INotesRepository _notesRepository;

        public NotesControllerTests()
        {
            _logger = A.Fake<ILogger<NotesController>>();
            _languageService = A.Fake<ILanguageService>();
            _notesRepository = A.Fake<INotesRepository>();

            _notesController = new NotesController(_logger, _languageService, _notesRepository);
        }

        [Fact]
        public void NotesController_Index_ReturnsSuccess()
        {
            //Arrange
            var notes = A.Fake<IEnumerable<NoteSummaryViewModel>>();
            var count = 0;
            var page = 1;
            var pageSize = 6;
            var offset = (page - 1) * pageSize;

            A.CallTo(() => _notesRepository.GetNotesSummaries(offset, pageSize)).Returns(notes);
            A.CallTo(() => _notesRepository.GetCount()).Returns(count);

            //Act
            var result = _notesController.Index(page, pageSize);

            //Assert
            result.Should().BeOfType<Task<IActionResult>>();
        }

        [Fact]
        public void NotesControlles_Details_ReturnsSuccess()
        {
            //Arrange
            var noteId = "1";
            var note = A.Fake<NoteDetailsViewModel>();

            A.CallTo(() => _notesRepository.GetNoteDetails(noteId)).Returns(note);

            //Act
            var result = _notesController.Details(noteId);

            //Assert
            result.Should().BeOfType<Task<IActionResult>>();
        }
    }
}