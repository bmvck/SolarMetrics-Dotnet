namespace SolarMetrics.Infrastructure.Mongo;

public interface IChatbotInteractionRepository
{
    bool IsEnabled { get; }

    Task<(IReadOnlyList<ChatbotInteractionDocument> Items, long TotalCount)> ListRecentAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task EnsureIndexesAsync(CancellationToken cancellationToken = default);
}
