namespace MyWebApp.Models
{
    public sealed class NotesMarkedByTag
    {
        public NotesMarkedByTag(string tag, IEnumerable<TagsAndScoreOfNote> notes)
        {
            Tag = tag;
            Notes = notes;
        }

        public string Tag { get; }
        public IEnumerable<TagsAndScoreOfNote> Notes { get; }
    }
}