using SolarMetrics.Web.Models;

namespace SolarMetrics.Web.Services;

public interface IOracleChatbotService
{
    Task<ChatbotAskResult> AskAsync(string? question, CancellationToken cancellationToken = default);
}
