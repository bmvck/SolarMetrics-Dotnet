using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SolarMetrics.Common;
using SolarMetrics.Infrastructure.Persistence.Entitites;
using SolarMetrics.Infrastructure.Persistence.Queries;

namespace SolarMetrics.Infrastructure.Persistence.Repositories;

public sealed class MonitoramentoRepository(SolarMetricsContext context) : IMonitoramentoRepository
{
    private static readonly Dictionary<string, Expression<Func<Monitoramento, object>>> SortMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["periodo"] = m => m.Periodo,
        ["valorlido"] = m => m.ValorLido,
        ["medialeitura"] = m => m.MediaLeitura,
        ["maximaleitura"] = m => m.MaximaLeitura
    };

    public async Task<(List<Monitoramento> Items, int TotalCount)> GetPagedAsync(
        MonitoramentoListQuery query,
        CancellationToken cancellationToken = default)
    {
        query.Normalize();
        IQueryable<Monitoramento> q = context.Monitoramentos.AsNoTracking()
            .Include(m => m.Sensor)
            .ThenInclude(s => s!.Sistema)
            .ThenInclude(sys => sys!.Cliente);

        if (query.SensorId.HasValue)
            q = q.Where(m => m.SensorId == query.SensorId.Value);
        if (!string.IsNullOrWhiteSpace(query.Periodo))
        {
            if (DateTime.TryParse(query.Periodo, out var periodo))
                q = q.Where(m => m.Periodo.Date == periodo.Date);
        }

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .ApplySort(query.SortBy, query.SortDir, SortMap, m => m.Periodo)
            .Skip(query.Skip)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<Monitoramento?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Monitoramentos
            .Include(m => m.Sensor)
            .ThenInclude(s => s!.Sistema)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<Monitoramento> AddAsync(Monitoramento monitoramento, CancellationToken cancellationToken = default)
    {
        context.Monitoramentos.Add(monitoramento);
        await context.SaveChangesAsync(cancellationToken);
        return monitoramento;
    }

    public async Task DeleteAsync(Monitoramento monitoramento, CancellationToken cancellationToken = default)
    {
        context.Monitoramentos.Remove(monitoramento);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
