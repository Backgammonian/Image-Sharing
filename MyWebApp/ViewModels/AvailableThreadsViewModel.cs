using MyWebApp.Models;

namespace MyWebApp.ViewModels
{
    public sealed class AvailableThreadsViewModel
    {
        public IEnumerable<ThreadModel> Threads { get; set; } = Enumerable.Empty<ThreadModel>();
    }
}
