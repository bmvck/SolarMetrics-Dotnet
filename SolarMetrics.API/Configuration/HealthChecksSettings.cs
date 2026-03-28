namespace SolarMetrics.Configuration;

public class HealthChecksSettings
{
    public const string SectionName = "HealthChecks";

    /// <summary>URL HTTP(S) para verificar disponibilidade de serviço externo (ex.: https://httpbin.org/get).</summary>
    public string ExternalUrl { get; set; } = "https://httpbin.org/get";

    public int ExternalTimeoutSeconds { get; set; } = 5;
}
