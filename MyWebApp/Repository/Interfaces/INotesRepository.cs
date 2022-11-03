using MyWebApp.Models;

namespace MyWebApp.Repository.Interfaces
{
    public interface INotesRepository
    {
        Task<IEnumerable<NoteSummary>> GetNotesList();
        Task<NoteDetails?> GetNoteDetails(string noteId);
    }
}
