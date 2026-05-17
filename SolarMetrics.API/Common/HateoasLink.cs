namespace SolarMetrics.Common;

public sealed class HateoasLink
{
    public string Rel { get; set; } = null!;
    public string Href { get; set; } = null!;
    public string? Method { get; set; }
}
