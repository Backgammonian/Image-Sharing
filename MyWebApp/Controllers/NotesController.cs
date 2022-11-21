using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Index()
        {
            var notes = await _notesRepository.GetNotesList();
            return View(notes);
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
            return View();
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

            await _notesRepository.Create(createNoteVM);
            return RedirectToAction("Index");
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
                Title = note.Title,
                Description = note.Description
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

            await _notesRepository.Update(originalNote, editNoteVM);
            return RedirectToAction("Index");
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

            await _notesRepository.Delete(deleteNoteVM);
            return RedirectToAction("Index");
        }
    }
}
