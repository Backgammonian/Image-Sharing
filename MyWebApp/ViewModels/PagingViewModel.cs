namespace MyWebApp.ViewModels
{
    public sealed class PagingViewModel
    {
        public int PageSize { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }

        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
        public int ResultStart => ((Page - 1) * PageSize) + 1;
        public int ResultEnd => Math.Min(ResultStart + PageSize - 1, TotalItems);
        public bool IsValidPage => ResultStart <= TotalItems;
        public string PrevDisabled => !HasPreviousPage ? "disabled" : string.Empty;
        public string NextDisabled => !HasNextPage ? "disabled" : string.Empty;
    }
}
