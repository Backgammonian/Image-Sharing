using MyWebApp.Models;

namespace MyWebApp.ViewModels
{
    public class AvailableThreadsViewModel
    {
        public IEnumerable<ThreadModel> Threads { get; set; } = Enumerable.Empty<ThreadModel>();
    }
}
