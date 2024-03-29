﻿namespace MyWebApp.ViewModels
{
    public class NoteSummariesListViewModel
    {
        public IEnumerable<NoteSummaryViewModel> NotesSummaries { get; set; } = Enumerable.Empty<NoteSummaryViewModel>();
        public PagingViewModel PagingViewModel { get; set; } = new PagingViewModel();
    }
}
