using Microsoft.AspNetCore.Mvc;
using MyWebApp.Models;
using MyWebApp.Repository.Interfaces;

namespace MyWebApp.Controllers
{
    public sealed class NotesController : Controller
    {
        private readonly INotesRepository _notesRepository;

        public NotesController(INotesRepository notesRepository)
        {
            _notesRepository = notesRepository;
        }

        [HttpGet]
        [Route("Notes")]
        public async Task<IActionResult> Index()
        {
            return View(await _notesRepository.GetNotesList());
        }

        [HttpGet]
        [Route("Notes/Details/{noteId}")]
        public async Task<IActionResult> Details(string? noteId)
        {
            return View(await _notesRepository.GetNoteDetails(noteId));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateNoteViewModel createNoteVM)
        {
            if (!ModelState.IsValid)
            {
                return View(createNoteVM);
            }

            await _notesRepository.Create(createNoteVM);
            return RedirectToAction("Index");
        }
    }
}
