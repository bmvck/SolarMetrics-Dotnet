using Microsoft.EntityFrameworkCore;
using SolarMetrics.Web.Models;

namespace SolarMetrics.Web.Repositories;

public sealed class SensorRepository(SolarMetricsContext context) : ISensorRepository
{
    public async Task<List<Sensor>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Sensores
            .AsNoTracking()
            .Include(s => s.Sistema)
            .ThenInclude(si => si!.Cliente)
            .OrderBy(s => s.Tipo)
            .ToListAsync(cancellationToken);
    }

    public async Task<Sensor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Sensores
            .Include(s => s.Sistema)
            .ThenInclude(si => si!.Cliente)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

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

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
