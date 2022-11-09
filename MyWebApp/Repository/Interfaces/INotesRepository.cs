using Microsoft.EntityFrameworkCore;
using MyWebApp.Models;
using MyWebApp.TableModels;
using MyWebApp.ViewModels;

namespace MyWebApp.Repository.Interfaces
{
    public interface INotesRepository
    {
        Task<IEnumerable<NoteSummary>> GetNotesList();
        Task<NoteDetails?> GetNoteDetails(string noteId);
        Task<NoteModel?> GetNoteModel(string noteId);
        Task<NoteModel?> GetNoteModelNoTracking(string noteId);
        Task<bool> Create(CreateNoteViewModel createNoteVM);
        Task<bool> Update(NoteModel note, EditNoteViewModel editNoteVM);
        Task<bool> Delete(NoteModel note);
        Task<bool> Save();
    }
}
