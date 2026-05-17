using SolarMetrics.Infrastructure.Persistence.Entitites;
using SolarMetrics.Infrastructure.Persistence.Queries;

namespace SolarMetrics.UseCase;

public interface ISensorUseCase
{
    Task<(List<Sensor> Items, int TotalCount)> GetPagedAsync(SensorListQuery query, CancellationToken cancellationToken = default);
    Task<Sensor> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Sensor> CreateAsync(Sensor sensor, CancellationToken cancellationToken = default);
    Task<Sensor> UpdateAsync(Guid id, Sensor dados, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
