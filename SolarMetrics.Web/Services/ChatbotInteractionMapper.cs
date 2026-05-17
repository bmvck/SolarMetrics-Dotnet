using System.Security.Claims;
using SolarMetrics.Web.Models;

namespace SolarMetrics.Web.Services;

public static class ChatbotInteractionMapper
{
    private const int MaxUserAgentLength = 512;

    public static ChatbotInteractionDocument FromAskResult(
        ChatbotAskResult result,
        string? questionFromRequest,
        long durationMs,
        ClaimsPrincipal? user,
        string? clientIp,
        string? userAgent)
    {
        var question = (result.Question ?? questionFromRequest ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(question))
            question = (questionFromRequest ?? string.Empty).Trim();

        return new ChatbotInteractionDocument
        {
            CreatedAtUtc = DateTime.UtcNow,
            UserId = user?.FindFirstValue(ClaimTypes.NameIdentifier),
            UserName = user?.FindFirstValue(ClaimTypes.Name),
            Question = question,
            Answer = result.Answer,
            Source = result.Source,
            Success = result.Success,
            ErrorMessage = result.ErrorMessage,
            DurationMs = durationMs,
            ClientIp = clientIp,
            UserAgent = TruncateUserAgent(userAgent)
        };
    }

    public static ChatbotInteractionListItem ToListItem(ChatbotInteractionDocument document) =>
        new()
        {
            Id = document.Id ?? string.Empty,
            CreatedAtUtc = document.CreatedAtUtc,
            UserName = document.UserName,
            Question = document.Question,
            Answer = document.Answer,
            Source = document.Source,
            Success = document.Success,
            ErrorMessage = document.ErrorMessage,
            DurationMs = document.DurationMs
        };

    internal static string? TruncateUserAgent(string? userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
            return userAgent;

        return userAgent.Length <= MaxUserAgentLength
            ? userAgent
            : userAgent[..MaxUserAgentLength];
    }
}
