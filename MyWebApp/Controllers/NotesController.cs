using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyWebApp.Repository;
using MyWebApp.ViewModels;

namespace MyWebApp.Controllers
{
    public sealed class NotesController : Controller
    {
        private readonly NotesRepository _notesRepository;

        public NotesController(NotesRepository notesRepository)
        {
            _notesRepository = notesRepository;
        }

        [HttpGet]
        [Route("Notes")]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 6)
        {
            if (page < 1 ||
                pageSize < 1)
            {
                return NotFound();
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
                ModelState.AddModelError(string.Empty, "Failed to create a new note");

                return View("Create", createNoteVM);
            }

            if (await _notesRepository.Create(createNoteVM))
            {
                return RedirectToAction("Index");
            }
          
            ModelState.AddModelError(string.Empty, "You are not logged in!");

            return View("Create", createNoteVM);
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
                ModelState.AddModelError(string.Empty, "Failed to edit the note");

                return View("Edit", editNoteVM);
            }

            var originalNote = await _notesRepository.GetNoteNoTracking(noteId);
            if (originalNote == null)
            {
                return View("Error");
            }

            if (await _notesRepository.Update(originalNote, editNoteVM))
            {
                return RedirectToAction("Index");
            }
          
            ModelState.AddModelError(string.Empty, "You have no permission to edit this note.");

            return View("Edit", editNoteVM);
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
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, "You have no permission to delete this note.");

            return View("Delete", deleteNoteVM);
        }
    }
}
