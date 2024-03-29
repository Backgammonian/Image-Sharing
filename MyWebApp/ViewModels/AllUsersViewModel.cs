﻿namespace MyWebApp.ViewModels
{
    public class AllUsersViewModel
    {
        public IEnumerable<UserSummaryViewModel> Users { get; set; } = Enumerable.Empty<UserSummaryViewModel>();
        public PagingViewModel PagingViewModel { get; set; } = new PagingViewModel();
    }
}
