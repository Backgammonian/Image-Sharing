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
        private readonly ILogger<NotesController> _logger;
        private readonly ILanguageService _languageService;
        private readonly INotesRepository _notesRepository;
        private readonly ICredentialsRepository _credentialsRepository;
        private readonly NotesController _notesController;

        public NotesControllerTests()
        {
            _logger = A.Fake<ILogger<NotesController>>();
            _languageService = A.Fake<ILanguageService>();
            _notesRepository = A.Fake<INotesRepository>();
            _credentialsRepository = A.Fake<ICredentialsRepository>();
            _notesController = new NotesController(_logger, _languageService, _notesRepository, _credentialsRepository);
        }

        [Fact]
        public async Task NotesController_Index_ReturnsSuccess()
        {
            var notes = A.Fake<IEnumerable<NoteSummaryViewModel>>();
            var count = 0;
            var page = 1;
            var pageSize = 6;
            var offset = (page - 1) * pageSize;
            A.CallTo(() => _notesRepository.GetNotesSummaries(offset, pageSize)).Returns(notes);
            A.CallTo(() => _notesRepository.GetCount()).Returns(count);

            var result = await _notesController.Index(page, pageSize);

            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task NotesController_Details_ReturnsSuccess()
        {
            var noteId = "1";
            var note = A.Fake<NoteDetailsViewModel>();
            A.CallTo(() => _notesRepository.GetNoteDetails(noteId)).Returns(note);

            var result = await _notesController.Details(noteId);

            result.Should().NotBeNull();
            result.Should().BeOfType<ViewResult>();
        }
    }
}