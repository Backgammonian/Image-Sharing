﻿using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.ViewModels;

namespace MyWebApp.Repository
{
    public sealed class ThreadsRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly NotesRepository _notesRepository;

        public ThreadsRepository(ApplicationDbContext dbContext,
            NotesRepository notesRepository)
        {
            _dbContext = dbContext;
            _notesRepository = notesRepository;
        }

        public async Task<IEnumerable<NoteThreadModel>> GetNotesFromThread(string thread)
        {
            return await _dbContext.NoteThreads.AsNoTracking().Where(x => x.Thread == thread).ToListAsync();
        }

        public async Task<IEnumerable<ThreadModel>> GetAllThreads()
        {
            return await _dbContext.Threads.AsNoTracking().ToListAsync();
        }

        public async Task<NotesFromThreadViewModel> GetByThread(string thread)
        {
            var notesFromThread = await GetNotesFromThread(thread);

            var notes = new List<NoteModel>();
            foreach (var noteFromThread in notesFromThread)
            {
                var note = await _notesRepository.GetNoteNoTracking(noteFromThread.NoteId);
                if (note != null)
                {
                    notes.Add(note);
                }
            }

            var threadNotesDetailsList = new List<NoteDetailsViewModel>();
            foreach (var noteFromThread in notesFromThread)
            {
                var note = await _notesRepository.GetNoteNoTracking(noteFromThread.NoteId);
                if (note != null)
                {
                    var noteDetails = await _notesRepository.GetNoteDetails(note.NoteId);
                    if (noteDetails != null)
                    {
                        threadNotesDetailsList.Add(noteDetails);
                    }
                }
            }

            return new NotesFromThreadViewModel()
            {
                Thread = thread,
                NotesDetails = threadNotesDetailsList
            };
        }

        public async Task<bool> Create(CreateThreadViewModel createThreadVM)
        {
            var newThreadName = createThreadVM.NewThreadName.ToLower();
            var allThreads = await GetAllThreads();
            if (allThreads.Any(x => x.Thread == newThreadName)) 
            {
                return false;
            }

            await _dbContext.Threads.AddAsync(new ThreadModel()
            {
                Thread = newThreadName
            });

            return await Save();
        }

        public async Task<bool> Delete(DeleteThreadViewModel deleteThreadVM)
        {
            var threadName = deleteThreadVM.SelectedThreadName.ToLower();
            var allThreads = await GetAllThreads();
            if (!allThreads.Any(x => x.Thread == threadName))
            {
                return false;
            }

            _dbContext.Threads.Remove(new ThreadModel()
            {
                Thread = threadName
            });

            var noteThreads = await _dbContext.NoteThreads.AsNoTracking().Where(x => x.Thread == threadName).ToListAsync();
            foreach (var noteThread in noteThreads)
            {
                _dbContext.NoteThreads.Remove(noteThread);
            }

            return await Save();
        }

        public async Task<bool> Save()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
