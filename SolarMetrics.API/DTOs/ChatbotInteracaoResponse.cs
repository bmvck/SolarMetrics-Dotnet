using SolarMetrics.Infrastructure.Mongo;

namespace SolarMetrics.DTOs;

public sealed class ChatbotInteracaoResponse
{
    public string? Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string Question { get; set; } = string.Empty;
    public string? Answer { get; set; }
    public string? Source { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public long DurationMs { get; set; }

    public static ChatbotInteracaoResponse FromDocument(ChatbotInteractionDocument doc) => new()
    {
        Id = doc.Id,
        CreatedAtUtc = doc.CreatedAtUtc,
        UserId = doc.UserId,
        UserName = doc.UserName,
        Question = doc.Question,
        Answer = doc.Answer,
        Source = doc.Source,
        Success = doc.Success,
        ErrorMessage = doc.ErrorMessage,
        DurationMs = doc.DurationMs
    };
}
