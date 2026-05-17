namespace SolarMetrics.Web.Configuration;

public sealed class ChatbotSettings
{
    public const string SectionName = "Chatbot";

    /// <summary>URL completa do endpoint ORDS POST /ords/admin/chatbot/ask.</summary>
    public string OrdsAskUrl { get; set; } = string.Empty;

    /// <summary>Timeout em segundos para chamadas ao Oracle Select AI (pode demorar).</summary>
    public int TimeoutSeconds { get; set; } = 120;
}
