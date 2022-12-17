using MyWebApp.Data;
using MyWebApp.Extensions;
using MyWebApp.Models;
using MyWebApp.ViewModels;

namespace MyWebApp.Repository
{
    public sealed class RatingsRepository
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ApplicationDbContext _dbContext;
        private readonly NotesRepository _notesRepository;
        private readonly UsersRepository _usersRepository;

        public RatingsRepository(IHttpContextAccessor contextAccessor,
            ApplicationDbContext dbContext,
            NotesRepository notesRepository,
            UsersRepository usersRepository)
        {
            _contextAccessor = contextAccessor;
            _dbContext = dbContext;
            _notesRepository = notesRepository;
            _usersRepository = usersRepository;
        }

        private async Task<UserModel?> TryGetUser()
        {
            var currentUser = _contextAccessor.HttpContext?.User;
            if (currentUser == null)
            {
                return null;
            }

            if (!currentUser.IsAuthenticated())
            {
                return null;
            }

            var currentUserId = currentUser.GetUserId();
            if (currentUserId == string.Empty)
            {
                return null;
            }

            return await _usersRepository.GetUserNoTracking(currentUserId);
        }

        public async Task<RatingAppliedViewModel?> VoteUp(string noteId)
        {
            var user = await TryGetUser();
            if (user == null)
            {
                return null;
            }

            if (noteId.IsEmpty())
            {
                return null;
            }

            var note = await _notesRepository.GetNoteNoTracking(noteId);
            if (note == null)
            {
                return null;
            }

            return new RatingAppliedViewModel();
        }
'
        public async Task<RatingAppliedViewModel?> VoteDown(string noteId)
        {
            var user = await TryGetUser();
            if (user == null)
            {
                return null;
            }

            if (noteId.IsEmpty())
            {
                return null;
            }

            var note = _notesRepository.GetNoteNoTracking(noteId);
            if (note == null)
            {
                return null;
            }

            return new RatingAppliedViewModel();
        }
    }
}
