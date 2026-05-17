namespace SolarMetrics.Infrastructure.Mongo;

public sealed class NullChatbotInteractionRepository : IChatbotInteractionRepository
{
    public bool IsEnabled => false;

    public Task<(IReadOnlyList<ChatbotInteractionDocument> Items, long TotalCount)> ListRecentAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<(IReadOnlyList<ChatbotInteractionDocument> Items, long TotalCount)>(([], 0));

    public Task EnsureIndexesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
