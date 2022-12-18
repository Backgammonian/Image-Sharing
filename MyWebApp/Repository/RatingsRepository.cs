using Microsoft.EntityFrameworkCore;
using MyWebApp.Data;
using MyWebApp.Extensions;
using MyWebApp.Models;
using MyWebApp.ViewModels;

namespace MyWebApp.Repository
{
    public enum VoteType
    {
        UpVote,
        DownVote
    }

    public sealed class RatingsRepository
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly RandomGenerator _randomGenerator;
        private readonly ApplicationDbContext _dbContext;
        private readonly NotesRepository _notesRepository;
        private readonly UsersRepository _usersRepository;
        private readonly MyUrlHelper _urlHelper;

        public RatingsRepository(IHttpContextAccessor contextAccessor,
            RandomGenerator randomGenerator,
            ApplicationDbContext dbContext,
            NotesRepository notesRepository,
            UsersRepository usersRepository,
            MyUrlHelper urlHelper)
        {
            _contextAccessor = contextAccessor;
            _randomGenerator = randomGenerator;
            _dbContext = dbContext;
            _notesRepository = notesRepository;
            _usersRepository = usersRepository;
            _urlHelper = urlHelper;
        }

        public async Task<UserModel?> GetUser()
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

        public async Task<NoteModel?> GetNote(string noteId)
        {
            if (noteId.IsEmpty())
            {
                return null;
            }

            var note = await _notesRepository.GetNoteNoTracking(noteId);
            if (note == null)
            {
                return null;
            }

            return note;
        }

        public async Task<RatingModel?> GetRatingOfNoteByUser(UserModel user, NoteModel note)
        {
            var userRatings = await _dbContext.Ratings.AsNoTracking().Where(x => x.UserId == user.Id).ToListAsync();
            foreach (var userRating in userRatings)
            {
                if (userRating.NoteId == note.NoteId)
                {
                    return userRating;
                }
            }

            return null;
        }

        public async Task<RatingModel> VoteForNote(UserModel user, NoteModel note, VoteType type)
        {
            var vote = new RatingModel()
            {
                RatingId = _randomGenerator.GetRandomId(),
                UserId = user.Id,
                NoteId = note.NoteId,
                Score = type switch
                {
                    VoteType.UpVote => 1,
                    VoteType.DownVote => -1,
                    _ => 0
                }
            };

            await _dbContext.Ratings.AddAsync(vote);

            return vote;
        }

        public void RemoveRating(RatingModel rating)
        {
            _dbContext.Ratings.Remove(rating);
        }

        public async Task<RatingAppliedViewModel> Vote(string noteId, VoteType voteType)
        {
            var url = _urlHelper.GetCurrentUrl();
            var ratingAppliedViewModel = new RatingAppliedViewModel(url);

            var user = await GetUser();
            if (user == null)
            {
                return ratingAppliedViewModel;
            }

            ratingAppliedViewModel.VotingUser = user;

            var note = await GetNote(noteId);
            if (note == null)
            {
                return ratingAppliedViewModel;
            }

            ratingAppliedViewModel.Note = note;

            var existingRating = await GetRatingOfNoteByUser(user, note);
            if (existingRating == null)
            {
                ratingAppliedViewModel.Rating = await VoteForNote(user, note, voteType);

                return ratingAppliedViewModel;
            }

            //Negating the exisiting rating
            var existingVoteType = existingRating.Score == 1 ? VoteType.UpVote : VoteType.DownVote;
            if (existingVoteType == voteType)
            {
                RemoveRating(existingRating);
                var negatedVoteType = existingVoteType == VoteType.UpVote ? VoteType.DownVote : VoteType.UpVote;
                ratingAppliedViewModel.Rating = await VoteForNote(user, note, negatedVoteType);

                return ratingAppliedViewModel;
            }

            //User has already made the same vote
            //Just show some kind of error in the View
            return ratingAppliedViewModel;
        }

        public async Task<RatingAppliedViewModel> VoteUp(string noteId)
        {
            return await Vote(noteId, VoteType.UpVote);
        }

        public async Task<RatingAppliedViewModel> VoteDown(string noteId)
        {
            return await Vote(noteId, VoteType.DownVote);
        }
    }
}
