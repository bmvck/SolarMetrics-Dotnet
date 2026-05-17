using System.Linq.Expressions;

namespace SolarMetrics.Common;

public static class QueryablePagingExtensions
{
    public static IQueryable<T> ApplySort<T>(
        this IQueryable<T> query,
        string? sortBy,
        string sortDir,
        IReadOnlyDictionary<string, Expression<Func<T, object>>> sortMap,
        Expression<Func<T, object>> defaultSort)
    {
        var key = string.IsNullOrWhiteSpace(sortBy)
            ? null
            : sortBy.Trim().ToLowerInvariant();

        if (key == null || !sortMap.TryGetValue(key, out var selector))
            selector = defaultSort;

        return string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase)
            ? query.OrderByDescending(selector)
            : query.OrderBy(selector);
    }
}
