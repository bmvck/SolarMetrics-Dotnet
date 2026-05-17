using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SolarMetrics.Common;
using SolarMetrics.Infrastructure.Persistence.Entitites;
using SolarMetrics.Infrastructure.Persistence.Queries;

namespace SolarMetrics.Infrastructure.Persistence.Repositories;

public sealed class SensorRepository(SolarMetricsContext context) : ISensorRepository
{
    private static readonly Dictionary<string, Expression<Func<Sensor, object>>> SortMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["tipo"] = s => s.Tipo,
        ["status"] = s => s.Status,
        ["localizacao"] = s => s.Localizacao ?? string.Empty
    };

    public async Task<(List<Sensor> Items, int TotalCount)> GetPagedAsync(
        SensorListQuery query,
        CancellationToken cancellationToken = default)
    {
        query.Normalize();
        IQueryable<Sensor> q = context.Sensores.AsNoTracking()
            .Include(s => s.Sistema)
            .ThenInclude(sys => sys!.Cliente);

        if (!string.IsNullOrWhiteSpace(query.Tipo))
            q = q.Where(s => s.Tipo.Contains(query.Tipo));
        if (!string.IsNullOrWhiteSpace(query.Status))
            q = q.Where(s => s.Status == query.Status);
        if (query.SistemaId.HasValue)
            q = q.Where(s => s.SistemaId == query.SistemaId.Value);

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .ApplySort(query.SortBy, query.SortDir, SortMap, s => s.Tipo)
            .Skip(query.Skip)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<Sensor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Sensores
            .Include(s => s.Sistema)
            .ThenInclude(sys => sys!.Cliente)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<Sensor> AddAsync(Sensor sensor, CancellationToken cancellationToken = default)
    {
        context.Sensores.Add(sensor);
        await context.SaveChangesAsync(cancellationToken);
        return sensor;
    }

    public async Task DeleteAsync(Sensor sensor, CancellationToken cancellationToken = default)
    {
        context.Sensores.Remove(sensor);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
