using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SolarMetrics.Common;
using SolarMetrics.Infrastructure.Persistence.Entitites;
using SolarMetrics.Infrastructure.Persistence.Queries;

namespace SolarMetrics.Infrastructure.Persistence.Repositories;

public sealed class PainelSolarRepository(SolarMetricsContext context) : IPainelSolarRepository
{
    private static readonly Dictionary<string, Expression<Func<PainelSolar, object>>> SortMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["modelo"] = p => p.Modelo,
        ["fabricante"] = p => p.Fabricante,
        ["potenciamaxima"] = p => p.PotenciaMaxima,
        ["eficiencia"] = p => p.Eficiencia
    };

    public async Task<(List<PainelSolar> Items, int TotalCount)> GetPagedAsync(
        PainelSolarListQuery query,
        CancellationToken cancellationToken = default)
    {
        query.Normalize();
        IQueryable<PainelSolar> q = context.PaineisSolares.AsNoTracking()
            .Include(p => p.Sistema)
            .ThenInclude(s => s!.Cliente);

        if (!string.IsNullOrWhiteSpace(query.Modelo))
            q = q.Where(p => p.Modelo.Contains(query.Modelo));
        if (!string.IsNullOrWhiteSpace(query.Fabricante))
            q = q.Where(p => p.Fabricante.Contains(query.Fabricante));
        if (query.SistemaId.HasValue)
            q = q.Where(p => p.SistemaId == query.SistemaId.Value);

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .ApplySort(query.SortBy, query.SortDir, SortMap, p => p.Modelo)
            .Skip(query.Skip)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<PainelSolar?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.PaineisSolares
            .Include(p => p.Sistema)
            .ThenInclude(s => s!.Cliente)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<PainelSolar> AddAsync(PainelSolar painel, CancellationToken cancellationToken = default)
    {
        context.PaineisSolares.Add(painel);
        await context.SaveChangesAsync(cancellationToken);
        return painel;
    }

    public async Task DeleteAsync(PainelSolar painel, CancellationToken cancellationToken = default)
    {
        context.PaineisSolares.Remove(painel);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
