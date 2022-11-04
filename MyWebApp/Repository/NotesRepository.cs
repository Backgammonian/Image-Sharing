using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Models;
using MyWebApp.Repository.Interfaces;
using MyWebApp.TableModels;
using System.Diagnostics;

namespace MyWebApp.Repository
{
    public sealed class NotesRepository : INotesRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public NotesRepository(ApplicationDbContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            _dbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IEnumerable<NoteSummary>> GetNotesList()
        {
            var list = new List<NoteSummary>();
            var notes = await _dbContext.Notes.ToListAsync();
            foreach (var note in notes)
            {
                var noteScore = await _dbContext.Ratings.Where(x => x.NoteId == note.NoteId).SumAsync(x => x.Score);
                var noteTags = await _dbContext.TagsForNotes.Where(x => x.NoteId == note.NoteId).Select(x => x.Tag).ToListAsync();
                list.Add(new NoteSummary(note, noteScore, noteTags));
            }

            return list;
        }

        public async Task<NoteDetails?> GetNoteDetails(string? noteId)
        {
            if (noteId == null ||
                noteId == string.Empty)
            {
                return null;
            }

            var note = await _dbContext.Notes.FirstOrDefaultAsync(x => x.NoteId == noteId);
            if (note == null)
            {
                return null;
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserId == note.UserId);
            if (user == null)
            {
                return null;
            }

            var images = await _dbContext.Images.Where(x => x.NoteId == noteId).ToListAsync();
            var score = await _dbContext.Ratings.Where(x => x.NoteId == noteId).SumAsync(x => x.Score);
            var tags = await _dbContext.TagsForNotes.Where(x => x.NoteId == noteId).Select(x => x.Tag).ToListAsync();
            var noteDetails = new NoteDetails(note, images, user, score, tags);

            return noteDetails;
        }

        public async Task<bool> Create(CreateNoteViewModel createNoteVM)
        {
            //TODO
            var user = await _dbContext.Users.FirstOrDefaultAsync();
            var noteModel = new NoteModel()
            {
                NoteId = createNoteVM.NoteId,
                UserId = user.UserId, //PLACEHOLDER
                Title = createNoteVM.Title,
                Description = createNoteVM.Description,
            };
            await _dbContext.AddAsync(noteModel);

            var wwwRootPath = _webHostEnvironment.WebRootPath;

            Debug.WriteLine($"--------------------------(Create) wwwRootPath: {wwwRootPath}");
            Debug.WriteLine($"--------------------------(Create) createNoteVM.Images.Length: {createNoteVM.Images.Count}");

            foreach (var image in createNoteVM.Images)
            {
                var name = RandomGenerator.GetRandomString(80);
                var extension = Path.GetExtension(image.FileName);
                var newPath = Path.Combine(wwwRootPath + "/images", $"{name}{extension}");
                using var stream = new FileStream(newPath, FileMode.Create);
                await image.CopyToAsync(stream);

                Debug.WriteLine($"--------------------------(Create) newPath: {newPath}");

                var imageRecord = new ImageModel()
                {
                    ImageId = RandomGenerator.GetRandomId(),
                    NoteId = noteModel.NoteId,
                    ImagePath = newPath
                };
                await _dbContext.AddAsync(imageRecord);
            }

            return await Save();
        }

        public async Task<bool> Update(NoteModel note)
        {
            //TODO
            return await Save();
        }

        public async Task<bool> Delete(NoteModel note)
        {
            //TODO
            return await Save();
        }

        public async Task<bool> Save()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}