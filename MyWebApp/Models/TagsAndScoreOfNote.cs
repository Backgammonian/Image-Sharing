using MyWebApp.TableModels;

namespace MyWebApp.Models
{
    public sealed class TagsAndScoreOfNote
    {
        public TagsAndScoreOfNote(NoteModel note, int score, IEnumerable<string> tags)
        {
            Note = note;
            Score = score;
            Tags = tags;
        }

        public NoteModel Note { get; }
        public int Score { get; }
        public IEnumerable<string> Tags { get; }
    }
}
