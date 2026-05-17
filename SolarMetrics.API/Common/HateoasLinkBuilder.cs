using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SolarMetrics.Common;

public static class HateoasLinkBuilder
{
    public static List<HateoasLink> ForItem(
        HttpRequest request,
        string collectionPath,
        Guid id,
        bool includeMutations = true)
    {
        var baseUrl = GetBaseUrl(request);
        var selfHref = $"{baseUrl}{collectionPath}/{id}";
        var collectionHref = $"{baseUrl}{collectionPath}";

        var links = new List<HateoasLink>
        {
            Link("self", selfHref, "GET"),
            Link("collection", collectionHref, "GET")
        };

        if (includeMutations)
        {
            links.Add(Link("update", collectionHref, "PUT"));
            links.Add(Link("delete", selfHref, "DELETE"));
        }

        return links;
    }

    public static List<HateoasLink> ForCollection(
        HttpRequest request,
        string path,
        int page,
        int pageSize,
        int totalPages,
        IReadOnlyDictionary<string, string?> queryParams)
    {
        var baseUrl = GetBaseUrl(request);
        var links = new List<HateoasLink>
        {
            Link("self", BuildHref(baseUrl, path, page, pageSize, queryParams), "GET")
        };

        if (page > 1)
        {
            links.Add(Link("first", BuildHref(baseUrl, path, 1, pageSize, queryParams), "GET"));
            links.Add(Link("prev", BuildHref(baseUrl, path, page - 1, pageSize, queryParams), "GET"));
        }

        if (page < totalPages)
        {
            links.Add(Link("next", BuildHref(baseUrl, path, page + 1, pageSize, queryParams), "GET"));
            links.Add(Link("last", BuildHref(baseUrl, path, totalPages, pageSize, queryParams), "GET"));
        }

        return links;
    }

    public static PagedResource<T> ToPagedResource<T>(
        HttpRequest request,
        string path,
        IReadOnlyList<T> items,
        int page,
        int pageSize,
        int totalCount,
        IReadOnlyDictionary<string, string?>? extraQuery = null)
    {
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
        var query = extraQuery ?? new Dictionary<string, string?>();
        return new PagedResource<T>
        {
            Items = items.ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Links = ForCollection(request, path, page, pageSize, Math.Max(totalPages, 1), query)
        };
    }

    public static ApiResource<T> ToApiResource<T>(
        HttpRequest request,
        string collectionPath,
        Guid id,
        T data,
        bool includeMutations = true) =>
        new()
        {
            Data = data,
            Links = ForItem(request, collectionPath, id, includeMutations)
        };

    private static HateoasLink Link(string rel, string href, string method) =>
        new() { Rel = rel, Href = href, Method = method };

    private static string GetBaseUrl(HttpRequest request)
    {
        var pathBase = request.PathBase.HasValue ? request.PathBase.Value : string.Empty;
        return $"{request.Scheme}://{request.Host}{pathBase}";
    }

    private static string BuildHref(
        string baseUrl,
        string path,
        int page,
        int pageSize,
        IReadOnlyDictionary<string, string?> queryParams)
    {
        var pairs = queryParams
            .Where(kv => !string.Equals(kv.Key, "page", StringComparison.OrdinalIgnoreCase)
                         && !string.Equals(kv.Key, "pageSize", StringComparison.OrdinalIgnoreCase)
                         && !string.IsNullOrWhiteSpace(kv.Value))
            .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}")
            .ToList();

        pairs.Add($"page={page}");
        pairs.Add($"pageSize={pageSize}");
        return $"{baseUrl}{path}?{string.Join("&", pairs)}";
    }
}
