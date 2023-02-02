using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Data.Interfaces;
using MyWebApp.Models;
using MyWebApp.PicturesModule.Interfaces;
using MyWebApp.Repository.Interfaces;
using MyWebApp.ViewModels;

namespace MyWebApp.Repository
{
    public sealed class NotesRepository : INotesRepository
    {
        private readonly IRandomGenerator _randomGenerator;
        private readonly IPicturesLoader _picturesLoader;
        private readonly ApplicationDbContext _dbContext;

        public NotesRepository(IRandomGenerator randomGenerator,
            IPicturesLoader picturesLoader,
            ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _picturesLoader = picturesLoader;
            _randomGenerator = randomGenerator;
        }

        public async Task<IEnumerable<ThreadModel>> GetAvailableNoteThreads()
        {
            return await _dbContext.Threads
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<NoteModel>> GetNotesSlice(int offset, int size)
        {
            return await _dbContext.Notes
                .AsNoTracking()
                .Skip(offset)
                .Take(size)
                .ToListAsync();
        }

        public async Task<int> GetCount()
        {
            return await _dbContext.Notes
                .CountAsync();
        }

        public async Task<NoteModel?> GetNote(string noteId)
        {
            return await _dbContext.Notes
                .FirstOrDefaultAsync(x => x.NoteId == noteId);
        }

        public async Task<NoteModel?> GetNoteNoTracking(string noteId)
        {
            return await _dbContext.Notes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.NoteId == noteId);
        }

        public async Task<UserModel?> GetNoteAuthor(NoteModel? note)
        {
            if (note == null)
            {
                return null;
            }

            return await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == note.UserId);
        }

        public async Task<NoteThreadModel?> GetNoteThread(string noteId)
        {
            return await _dbContext.NoteThreads
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.NoteId == noteId);
        }

        public async Task<NoteImageModel?> GetNoteFirstImage(string noteId)
        {
            return await _dbContext.NoteImages
                .AsNoTracking()
                .Where(x => x.NoteId == noteId)
                .OrderBy(x => x.UploadTime)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<NoteImageModel>> GetNoteImages(string noteId)
        {
            return await _dbContext.NoteImages
                .AsNoTracking()
                .Where(x => x.NoteId == noteId)
                .OrderBy(x => x.UploadTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<NoteImageModel>> GetNoteImagesNoTracking(string noteId)
        {
            return await _dbContext.NoteImages
                .AsNoTracking()
                .Where(x => x.NoteId == noteId)
                .OrderBy(x => x.UploadTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<NoteSummaryViewModel>> GetNotesSummaries(int offset, int size)
        {
            var notes = await GetNotesSlice(offset, size);

            var noteSummaries = new List<NoteSummaryViewModel>();
            foreach (var note in notes)
            {
                var author = await GetNoteAuthor(note);
                var noteId = note.NoteId;
                var noteSummary = new NoteSummaryViewModel()
                {
                    Note = note,
                    Thread = await GetNoteThread(noteId),
                    FirstImage = await GetNoteFirstImage(noteId),
                    Author = author,
                    ProfilePicture = await _picturesLoader.GetUserCurrentProfilePicture(author)
                };

                noteSummaries.Add(noteSummary);
            }

            return noteSummaries;
        }

        public async Task<NoteDetailsViewModel> GetNoteDetails(string noteId)
        {
            var note = await GetNoteNoTracking(noteId);
            var author = await GetNoteAuthor(note);

            return new NoteDetailsViewModel()
            {
                Note = note,
                Thread = await GetNoteThread(noteId),
                Images = await GetNoteImagesNoTracking(noteId),
                Author = author,
                ProfilePicture = await _picturesLoader.GetUserCurrentProfilePicture(author),
            };
        }

        public async Task<string> Create(UserModel author, CreateNoteViewModel createNoteVM)
        {
            var note = new NoteModel()
            {
                NoteId = _randomGenerator.GetRandomId(),
                UserId = author.Id,
                Title = createNoteVM.Title,
                Description = createNoteVM.Description,
            };

            await _dbContext.Notes.AddAsync(note);

            foreach (var image in createNoteVM.Images)
            {
                var noteImage = await _picturesLoader.LoadNoteImage(image, note);
                await _dbContext.NoteImages.AddAsync(noteImage);
            }

            var selectedThread = createNoteVM.SelectedThread;
            if (selectedThread != string.Empty)
            {
                var availableThreads = await GetAvailableNoteThreads();
                if (availableThreads.Any(x => x.Thread == selectedThread))
                {
                    await _dbContext.NoteThreads.AddAsync(new NoteThreadModel()
                    {
                        ThreadId = selectedThread,
                        NoteId = note.NoteId
                    });
                }
            }

            await Save();

            return note.NoteId;
        }

        public async Task<bool> Update(string noteId, EditNoteViewModel editNoteVM)
        {
            var note = await GetNote(noteId);
            if (note == null)
            {
                return false;
            }

            var newImages = editNoteVM.Images;
            if (newImages.Any())
            {
                var oldImages = await GetNoteImages(note.NoteId);
                foreach (var oldImage in oldImages)
                {
                    _dbContext.NoteImages.Remove(oldImage);
                }

                foreach (var newImage in newImages)
                {
                    var newNoteImage = await _picturesLoader.LoadNoteImage(newImage, note);
                    await _dbContext.NoteImages.AddAsync(newNoteImage);
                }
            }

            var selectedThread = editNoteVM.SelectedThread;
            if (selectedThread != string.Empty)
            {
                var availableThreads = await GetAvailableNoteThreads();
                if (availableThreads.Any(x => x.Thread == selectedThread))
                {
                    var oldThreadOfNote = await GetNoteThread(note.NoteId);
                    if (oldThreadOfNote != null)
                    {
                        _dbContext.NoteThreads.Remove(oldThreadOfNote);
                    }

                    await _dbContext.NoteThreads.AddAsync(new NoteThreadModel()
                    {
                        ThreadId = selectedThread,
                        NoteId = note.NoteId
                    });
                }
            }

            note.Title = editNoteVM.Title;
            note.Description = editNoteVM.Description;
            _dbContext.Notes.Update(note);

            return await Save();
        }

        public async Task<bool> Delete(DeleteNoteViewModel deleteNoteVM)
        {
            var noteId = deleteNoteVM.NoteId;
            var note = await GetNote(noteId);
            if (note == null)
            {
                return false;
            }

            _dbContext.Notes.Remove(note);

            return await Save();
        }

        public async Task<bool> Save()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}