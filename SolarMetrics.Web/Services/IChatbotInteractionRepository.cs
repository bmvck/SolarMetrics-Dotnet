using SolarMetrics.Web.Models;

namespace SolarMetrics.Web.Services;

public interface IChatbotInteractionRepository
{
    Task InsertAsync(ChatbotInteractionDocument document, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<ChatbotInteractionDocument> Items, long TotalCount)> ListRecentAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task EnsureIndexesAsync(CancellationToken cancellationToken = default);
}
