using SolarMetrics.Web.Models;

namespace SolarMetrics.Web.Repositories;

public interface ISensorRepository
{
    Task<List<Sensor>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Sensor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Sensor> AddAsync(Sensor sensor, CancellationToken cancellationToken = default);
    Task DeleteAsync(Sensor sensor, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
