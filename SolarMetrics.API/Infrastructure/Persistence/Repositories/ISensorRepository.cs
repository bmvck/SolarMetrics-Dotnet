using SolarMetrics.Infrastructure.Persistence.Entitites;
using SolarMetrics.Infrastructure.Persistence.Queries;

namespace SolarMetrics.Infrastructure.Persistence.Repositories;

public interface ISensorRepository
{
    Task<(List<Sensor> Items, int TotalCount)> GetPagedAsync(SensorListQuery query, CancellationToken cancellationToken = default);
    Task<Sensor?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Sensor> AddAsync(Sensor sensor, CancellationToken cancellationToken = default);
    Task DeleteAsync(Sensor sensor, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
