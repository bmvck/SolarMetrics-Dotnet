using Microsoft.EntityFrameworkCore;
using SolarMetrics.Web.Models;

namespace SolarMetrics.Web.Repositories;

public sealed class MonitoramentoRepository(SolarMetricsContext context) : IMonitoramentoRepository
{
    public async Task<List<Monitoramento>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Monitoramentos
            .AsNoTracking()
            .Include(m => m.Sensor)
            .ThenInclude(s => s!.Sistema)
            .ThenInclude(si => si!.Cliente)
            .OrderByDescending(m => m.Periodo)
            .ToListAsync(cancellationToken);
    }

    public async Task<Monitoramento?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Monitoramentos
            .Include(m => m.Sensor)
            .ThenInclude(s => s!.Sistema)
            .ThenInclude(si => si!.Cliente)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

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

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
