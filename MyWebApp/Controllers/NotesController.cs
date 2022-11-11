﻿using Microsoft.AspNetCore.Mvc;
using MyWebApp.Repository.Interfaces;
using MyWebApp.ViewModels;
using System.Diagnostics;

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
            if (ModelState.IsValid)
            {
                await _notesRepository.Create(createNoteVM);
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, "Failed to create new note");
            return View(createNoteVM);
        }

        [HttpGet]
        [Route("Notes/Edit/{noteId}")]
        public async Task<IActionResult> Edit(string noteId)
        {
            var note = await _notesRepository.GetNoteModel(noteId);
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

            var originalNote = await _notesRepository.GetNoteModelNoTracking(noteId);
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
            var note = await _notesRepository.GetNoteModel(noteId);
            if (note == null)
            {
                return View("Error");
            }

            return View(note);
        }

        [HttpPost, ActionName(nameof(Delete))]
        [Route("Notes/Delete/{noteId}")]
        public async Task<IActionResult> DeleteNote(string noteId)
        {
            var note = await _notesRepository.GetNoteModelNoTracking(noteId);
            if (note == null)
            {
                return View("Error");
            }

            await _notesRepository.Delete(note);
            return RedirectToAction("Index");
        }
    }
}
