using System.Text.Json.Serialization;

namespace SolarMetrics.Common;

public sealed class PagedResource<T>
{
    public List<T> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    [JsonPropertyName("_links")]
    public List<HateoasLink> Links { get; set; } = [];
}
