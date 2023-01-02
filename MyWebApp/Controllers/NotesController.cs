using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyWebApp.Localization;
using MyWebApp.Repository;
using MyWebApp.ViewModels;

namespace MyWebApp.Controllers
{
    public sealed class NotesController : Controller
    {
        private readonly ILogger<NotesController> _logger;
        private readonly NotesRepository _notesRepository;
        private readonly LanguageService _languageService;

        public NotesController(ILogger<NotesController> logger,
            NotesRepository notesRepository,
            LanguageService languageService)
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
        public async Task<IActionResult> Create()
        {
            var availableThreads = await _notesRepository.GetAvailableNoteThreads();
            var selectedListItems = availableThreads.Select(x => new SelectListItem()
            {
                Value = x.Thread,
                Text = x.Thread,
            });

            var firstThread = selectedListItems.First();
            var createNoteViewModel = new CreateNoteViewModel()
            {
                AvailableThreads = selectedListItems,
                SelectedThread = firstThread != null ? firstThread.Value : string.Empty
            };

            return View(createNoteViewModel);
        }

        [HttpPost]
        [Route("Notes/Create")]
        public async Task<IActionResult> Create(CreateNoteViewModel createNoteVM)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = _languageService.GetKey("NoteCreate_WrongInput");

                return View(createNoteVM);
            }

            if (await _notesRepository.Create(createNoteVM))
            {
                _logger.LogInformation($"(Notes/Create) Note '{createNoteVM.Title}' has been created");

                return RedirectToAction("Index");
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

            var availableThreads = await _notesRepository.GetAvailableNoteThreads();
            var selectedListItems = availableThreads.Select(x => new SelectListItem()
            {
                Value = x.Thread,
                Text = x.Thread,
            });

            var threadOfNote = await _notesRepository.GetNoteThread(note.NoteId);
            if (threadOfNote != null)
            {
                foreach (var selectedListItem in selectedListItems)
                {
                    if (selectedListItem.Value == threadOfNote.Thread)
                    {
                        selectedListItem.Selected = true;
                        break;
                    }
                }
            }

            var editNoteVM = new EditNoteViewModel()
            {
                Title = note.Title,
                Description = note.Description,
                AvailableThreads = selectedListItems,
                SelectedThread = threadOfNote == null ? string.Empty : threadOfNote.Thread,
                ExistingImages = await _notesRepository.GetNoteImagesNoTracking(note.NoteId)
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
                _logger.LogInformation($"(Notes/Create) The note '{editNoteVM.Title}' ({originalNote.NoteId}) has been edited");

                return RedirectToAction("Index");
            }
            else
            {
                _logger.LogInformation($"(Notes/Create) Can't edit the note '{editNoteVM.Title}' ({originalNote.NoteId})");
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

            if (await _notesRepository.Delete(deleteNoteVM))
            {
                _logger.LogInformation($"(Notes/Delete) The note '{note.Title}' ({noteId}) has been deleted");

                return RedirectToAction("Index");
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
