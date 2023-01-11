using Microsoft.AspNetCore.Mvc;
using MyWebApp.Localization.Interfaces;
using MyWebApp.Repository.Interfaces;
using MyWebApp.ViewModels;

namespace MyWebApp.Controllers
{
    public sealed class NotesController : Controller
    {
        private readonly ILogger<NotesController> _logger;
        private readonly ILanguageService _languageService;
        private readonly INotesRepository _notesRepository;

        public NotesController(ILogger<NotesController> logger,
            ILanguageService languageService,
            INotesRepository notesRepository)
        {
            _logger = logger;
            _notesRepository = notesRepository;
            _languageService = languageService;
        }

        [HttpGet]
        [Route("Notes")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 6)
        {
            if (page < 1 ||
                pageSize < 1)
            {
                return RedirectToAction("ErrorWrongPage", "Error", new WrongPageViewModel()
                {
                    Page = page,
                    PageSize = pageSize
                });
            }

            var notes = await _notesRepository.GetNotesSummaries((page - 1) * pageSize, pageSize);
            var count = await _notesRepository.GetCount();

            return View(new NoteSummariesListViewModel()
            {
                NotesSummaries = notes,
                PagingViewModel = new PagingViewModel()
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalItems = count,
                    TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                }
            });
        }

        [HttpGet]
        [Route("Notes/Details/{noteId}")]
        public async Task<IActionResult> Details(string noteId)
        {
            var noteDetails = await _notesRepository.GetNoteDetails(noteId);

            return View(noteDetails);
        }

        [HttpGet]
        [Route("Notes/Create")]
        public IActionResult Create()
        {
            return View(new CreateNoteViewModel());
        }

        [HttpPost]
        [Route("Notes/Create")]
        public async Task<IActionResult> Create(CreateNoteViewModel createNoteVM)
        {
            _logger.LogInformation($"(Notes/Create) Note '{createNoteVM.Title}': selected thread: {createNoteVM.SelectedThread}");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = _languageService.GetKey("NoteCreate_WrongInput");

                return View(createNoteVM);
            }

            var createdNoteId = await _notesRepository.Create(createNoteVM);
            if (createdNoteId != string.Empty)
            {
                _logger.LogInformation($"(Notes/Create) Note '{createNoteVM.Title}' ({createdNoteId}) has been created");

                return RedirectToAction("Details", new { noteId = createdNoteId });
            }
            else
            {
                _logger.LogInformation($"(Notes/Create) Can't create a note '{createNoteVM.Title}'");
            }

            TempData["Error"] = _languageService.GetKey("NoteCreate_NotLoggedIn");

            return View(createNoteVM);
        }

        [HttpGet]
        [Route("Notes/Edit/{noteId}")]
        public async Task<IActionResult> Edit(string noteId)
        {
            var note = await _notesRepository.GetNote(noteId);
            if (note == null)
            {
                return View("Error");
            }

            var editNoteVM = new EditNoteViewModel()
            {
                NoteId = noteId,
                Title = note.Title,
                Description = note.Description,
            };

            return View(editNoteVM);
        }

        [HttpPost]
        [Route("Notes/Edit/{noteId}")]
        public async Task<IActionResult> Edit(string noteId, EditNoteViewModel editNoteVM)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = _languageService.GetKey("EditNote_WrongInput");

                return View(editNoteVM);
            }

            var originalNote = await _notesRepository.GetNoteNoTracking(noteId);
            if (originalNote == null)
            {
                return View("Error");
            }

            if (await _notesRepository.Update(originalNote, editNoteVM))
            {
                _logger.LogInformation($"(Notes/Create) The note '{editNoteVM.Title}' ({noteId}) has been edited");

                return RedirectToAction("Details", new { noteId });
            }
            else
            {
                _logger.LogInformation($"(Notes/Create) Can't edit the note '{editNoteVM.Title}' ({noteId})");
            }

            TempData["Error"] = _languageService.GetKey("EditNote_NoEditPermission");

            return View(editNoteVM);
        }

        [HttpGet]
        [Route("Notes/Delete/{noteId}")]
        public async Task<IActionResult> Delete(string noteId)
        {
            var note = await _notesRepository.GetNote(noteId);
            if (note == null)
            {
                return View("Error");
            }

            var deleteNoteVM = new DeleteNoteViewModel()
            {
                NoteId = note.NoteId,
                NoteDetails = await _notesRepository.GetNoteDetails(noteId)
            };

            _logger.LogInformation($"(Notes/Delete) The note '{note.Title}' ({noteId}) is preparing to be deleted!");

            return View(deleteNoteVM);
        }

        [HttpPost]
        [Route("Notes/Delete/{noteId}")]
        public async Task<IActionResult> Delete(string noteId, DeleteNoteViewModel deleteNoteVM)
        {
            var note = await _notesRepository.GetNoteNoTracking(noteId);
            if (note == null)
            {
                return View("Error");
            }

            if (await _notesRepository.Delete(note))
            {
                _logger.LogInformation($"(Notes/Delete) The note '{note.Title}' ({noteId}) has been deleted");

                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                _logger.LogInformation($"(Notes/Delete) Can't delete the note '{note.Title}' ({noteId})");
            }

            TempData["Error"] = _languageService.GetKey("DeleteNote_NoDeletePermission");

            return View(deleteNoteVM);
        }
    }
}