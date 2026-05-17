namespace SolarMetrics.Web.Models;

public sealed class ChatbotInteractionListItem
{
    public string Id { get; init; } = string.Empty;

    public DateTime CreatedAtUtc { get; init; }

    public string? UserName { get; init; }

    public string Question { get; init; } = string.Empty;

    public string? Answer { get; init; }

    public string? Source { get; init; }

    public bool Success { get; init; }

    public string? ErrorMessage { get; init; }

    public long DurationMs { get; init; }
}

public sealed class ChatbotHistoryViewModel
{
    public IReadOnlyList<ChatbotInteractionListItem> Items { get; init; } = Array.Empty<ChatbotInteractionListItem>();

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 50;

    public long TotalCount { get; init; }

    public int TotalPages => PageSize > 0
        ? (int)Math.Ceiling(TotalCount / (double)PageSize)
        : 0;

    public bool HasPreviousPage => Page > 1;

    public bool HasNextPage => Page < TotalPages;
}
