using Microsoft.AspNet.Identity;
using MyWebApp.Data;
using MyWebApp.ViewModels;

namespace MyWebApp.Repository
{
    public sealed class DashboardRepository
    {

        private readonly IHttpContextAccessor _contextAccessor;
        private readonly NotesRepository _notesRepository;
        private readonly UsersRepository _usersRepository;

        public DashboardRepository(IHttpContextAccessor contextAccessor,
            NotesRepository notesRepository,
            UsersRepository usersRepository)
        {
            _contextAccessor = contextAccessor;
            _notesRepository = notesRepository;
            _usersRepository = usersRepository;
        }

        public async Task<IEnumerable<NoteDetailsViewModel>> GetAllUserNotes()
        {
            var currentUser = _contextAccessor.HttpContext?.User;
            if (currentUser == null)
            {
                return Enumerable.Empty<NoteDetailsViewModel>();
            }

            var userId = currentUser.Identity.GetUserId();
            var userNotes = await _notesRepository.GetUsersNotes(userId);
            var notesDetails = new List<NoteDetailsViewModel>();
            foreach (var userNote in userNotes)
            {
                notesDetails.Add(await _notesRepository.GetNoteDetails(userNote.NoteId));
            }

            return notesDetails;
        }

        public async Task<UserRatingsViewModel?> GetAllUserRatings()
        {
            var currentUser = _contextAccessor.HttpContext?.User;
            if (currentUser == null)
            {
                return null;
            }

            var userId = currentUser.Identity.GetUserId();
            return await _usersRepository.GetUserRatings(userId);
        }
    }
}
