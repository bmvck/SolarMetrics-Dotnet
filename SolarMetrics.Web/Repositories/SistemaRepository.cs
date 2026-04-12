using Microsoft.EntityFrameworkCore;
using SolarMetrics.Web.Models;

namespace SolarMetrics.Web.Repositories;

public sealed class SistemaRepository(SolarMetricsContext context) : ISistemaRepository
{
    public async Task<List<Sistema>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Sistemas
            .AsNoTracking()
            .Include(s => s.Cliente)
            .OrderBy(s => s.NomeInstalacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<Sistema?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Sistemas
            .Include(s => s.Cliente)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

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

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
