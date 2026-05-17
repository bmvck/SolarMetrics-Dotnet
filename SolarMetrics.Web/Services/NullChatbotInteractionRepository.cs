using SolarMetrics.Web.Models;

namespace SolarMetrics.Web.Services;

public sealed class NullChatbotInteractionRepository : IChatbotInteractionRepository
{
    public Task InsertAsync(ChatbotInteractionDocument document, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;

    public Task<(IReadOnlyList<ChatbotInteractionDocument> Items, long TotalCount)> ListRecentAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default) =>
        Task.FromResult<(IReadOnlyList<ChatbotInteractionDocument>, long)>((Array.Empty<ChatbotInteractionDocument>(), 0));

    public Task EnsureIndexesAsync(CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
