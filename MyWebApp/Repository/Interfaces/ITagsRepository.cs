using MyWebApp.Models;

namespace MyWebApp.Repository.Interfaces
{
    public interface ITagsRepository
    {
        Task<NotesMarkedByTag?> GetByTag(string? tag);
    }
}
