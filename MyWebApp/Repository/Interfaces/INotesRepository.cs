using MyWebApp.Models;
using MyWebApp.ViewModels;

namespace MyWebApp.Repository.Interfaces
{
    public interface INotesRepository
    {
        Task<IEnumerable<ThreadModel>> GetAvailableNoteThreads();
        Task<IEnumerable<NoteModel>> GetNotesSlice(int offset, int size);
        Task<int> GetCount();
        Task<NoteModel?> GetNote(string noteId);
        Task<NoteModel?> GetNoteNoTracking(string noteId);
        Task<UserModel?> GetNoteAuthor(NoteModel? note);
        Task<NoteThreadModel?> GetNoteThread(string noteId);
        Task<NoteImageModel?> GetNoteFirstImage(string noteId);
        Task<IEnumerable<NoteImageModel>> GetNoteImages(string noteId);
        Task<IEnumerable<NoteImageModel>> GetNoteImagesNoTracking(string noteId);
        Task<IEnumerable<NoteSummaryViewModel>> GetNotesSummaries(int offset, int size);
        Task<NoteDetailsViewModel> GetNoteDetails(string noteId);
        Task<string> Create(UserModel author, CreateNoteViewModel createNoteVM);
        Task<bool> Update(NoteModel note, EditNoteViewModel editNoteVM);
        Task<bool> Delete(UserModel author, NoteModel note);
        Task<bool> Save();
    }
}
