using MyWebApp.Models;

namespace MyWebApp.ViewModels
{
    public sealed class AllUsersViewModel
    {
        public IEnumerable<UserSummaryViewModel> Users { get; set; } = Enumerable.Empty<UserSummaryViewModel>();
        public PagingViewModel PagingViewModel { get; set; } = new PagingViewModel();
    }
}
