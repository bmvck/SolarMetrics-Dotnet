using Microsoft.EntityFrameworkCore;
using SolarMetrics.Web.Models;

namespace SolarMetrics.Web.Repositories;

public sealed class PainelSolarRepository(SolarMetricsContext context) : IPainelSolarRepository
{
    public async Task<List<PainelSolar>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.PaineisSolares
            .AsNoTracking()
            .Include(p => p.Sistema)
            .ThenInclude(s => s!.Cliente)
            .OrderBy(p => p.Modelo)
            .ToListAsync(cancellationToken);
    }

    public async Task<PainelSolar?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.PaineisSolares
            .Include(p => p.Sistema)
            .ThenInclude(s => s!.Cliente)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

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

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
