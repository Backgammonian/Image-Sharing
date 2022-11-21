using MyWebApp.ViewModels;
using MyWebApp.Extensions;

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
            var currentUserId = _contextAccessor.HttpContext?.User.GetUserId();
            if (currentUserId == null ||
                currentUserId.IsEmpty())
            {
                return Enumerable.Empty<NoteDetailsViewModel>();
            }

            var userNotes = await _usersRepository.GetNotesOfUser(currentUserId);
            var notesDetails = new List<NoteDetailsViewModel>();
            foreach (var userNote in userNotes)
            {
                var noteDetail = await _notesRepository.GetNoteDetails(userNote.NoteId);
                notesDetails.Add(noteDetail);
            }

            return notesDetails;
        }

        public async Task<UserRatingsViewModel?> GetAllUserRatings()
        {
            var currentUserId = _contextAccessor.HttpContext?.User.GetUserId();
            if (currentUserId == null ||
                currentUserId.IsEmpty())
            {
                return null;
            }

            return await _usersRepository.GetUserRatings(currentUserId);
        }
    }
}
