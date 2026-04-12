namespace SolarMetrics.Web.Configuration;

public sealed class ApiSettings
{
    public const string SectionName = "Api";

    /// <summary>Base URL da SolarMetrics.API (ex.: http://localhost:5090) para obter JWT em /auth/token.</summary>
    public string BaseUrl { get; set; } = "http://localhost:5090";
}
