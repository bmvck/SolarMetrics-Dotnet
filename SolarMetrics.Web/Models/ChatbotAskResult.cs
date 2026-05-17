namespace SolarMetrics.Web.Models;

public sealed class ChatbotAskRequest
{
    public string? Question { get; set; }
}

public sealed class ChatbotAskResult
{
    public bool Success { get; init; }
    public string? Question { get; init; }
    public string? Answer { get; init; }
    public string? Source { get; init; }
    public string? ErrorMessage { get; init; }

    public static ChatbotAskResult Ok(string question, string answer, string? source) =>
        new()
        {
            Success = true,
            Question = question,
            Answer = answer,
            Source = source
        };

    public static ChatbotAskResult Fail(string message) =>
        new()
        {
            Success = false,
            ErrorMessage = message
        };
}
