using MyWebApp.TableModels;

namespace MyWebApp.Models
{
    public sealed class NoteSummaryViewModel
    {
        public NoteModel? Note { get; set; }
        public int Score { get; set; }
        public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();
        public string? FirstImageName { get; set; }
    }
}
