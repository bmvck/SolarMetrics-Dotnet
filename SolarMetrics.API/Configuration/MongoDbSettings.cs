namespace SolarMetrics.Configuration;

public sealed class MongoDbSettings
{
    public const string SectionName = "MongoDb";

    public bool Enabled { get; set; }
    public string? ConnectionString { get; set; }
    public string DatabaseName { get; set; } = "solarmetrics";
    public string ChatbotInteractionsCollection { get; set; } = "chatbot_interactions";
    public int HistoryPageSize { get; set; } = 50;
}
