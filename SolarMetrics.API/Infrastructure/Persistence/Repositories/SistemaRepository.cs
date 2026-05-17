using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SolarMetrics.Common;
using SolarMetrics.Infrastructure.Persistence.Entitites;
using SolarMetrics.Infrastructure.Persistence.Queries;

namespace SolarMetrics.Infrastructure.Persistence.Repositories;

public sealed class SistemaRepository(SolarMetricsContext context) : ISistemaRepository
{
    private static readonly Dictionary<string, Expression<Func<Sistema, object>>> SortMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["nomeinstalacao"] = s => s.NomeInstalacao,
        ["status"] = s => s.Status,
        ["datainstalacao"] = s => s.DataInstalacao,
        ["potenciatotal"] = s => s.PotenciaTotal
    };

    public async Task<(List<Sistema> Items, int TotalCount)> GetPagedAsync(
        SistemaListQuery query,
        CancellationToken cancellationToken = default)
    {
        query.Normalize();
        IQueryable<Sistema> q = context.Sistemas.AsNoTracking().Include(s => s.Cliente);

        if (!string.IsNullOrWhiteSpace(query.NomeInstalacao))
            q = q.Where(s => s.NomeInstalacao.Contains(query.NomeInstalacao));
        if (!string.IsNullOrWhiteSpace(query.Status))
            q = q.Where(s => s.Status == query.Status);
        if (query.ClienteId.HasValue)
            q = q.Where(s => s.ClienteId == query.ClienteId.Value);

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .ApplySort(query.SortBy, query.SortDir, SortMap, s => s.NomeInstalacao)
            .Skip(query.Skip)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<Sistema?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Sistemas.Include(s => s.Cliente).FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<Sistema> AddAsync(Sistema sistema, CancellationToken cancellationToken = default)
    {
        context.Sistemas.Add(sistema);
        await context.SaveChangesAsync(cancellationToken);
        return sistema;
    }

    public async Task DeleteAsync(Sistema sistema, CancellationToken cancellationToken = default)
    {
        context.Sistemas.Remove(sistema);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
