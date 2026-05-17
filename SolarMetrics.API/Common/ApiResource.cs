using System.Text.Json.Serialization;

namespace SolarMetrics.Common;

public sealed class ApiResource<T>
{
    public T Data { get; set; } = default!;

    [JsonPropertyName("_links")]
    public List<HateoasLink> Links { get; set; } = [];
}
