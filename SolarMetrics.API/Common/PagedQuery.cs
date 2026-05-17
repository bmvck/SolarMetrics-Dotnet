namespace SolarMetrics.Common;

public class PagedQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public string SortDir { get; set; } = "asc";

    public void Normalize(int maxPageSize = 100)
    {
        Page = Math.Max(1, Page);
        PageSize = Math.Clamp(PageSize <= 0 ? 20 : PageSize, 1, maxPageSize);
        SortDir = string.Equals(SortDir, "desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";
    }

    public int Skip => (Page - 1) * PageSize;
}
