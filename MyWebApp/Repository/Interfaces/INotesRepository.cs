using MyWebApp.Models;
using MyWebApp.TableModels;

namespace MyWebApp.Repository.Interfaces
{
    public interface INotesRepository
    {
        Task<IEnumerable<NoteSummary>> GetNotesList();
        Task<NoteDetails?> GetNoteDetails(string? noteId);
        Task<bool> Create(CreateNoteViewModel createNoteVM);
        Task<bool> Update(NoteModel note);
        Task<bool> Delete(NoteModel note);
        Task<bool> Save();
    }
}
