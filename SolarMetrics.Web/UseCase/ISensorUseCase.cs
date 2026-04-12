using SolarMetrics.Web.Models;

namespace SolarMetrics.Web.UseCase;

public interface ISensorUseCase
{
    Task<List<Sensor>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Sensor> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Sensor> CreateAsync(Sensor sensor, CancellationToken cancellationToken = default);
    Task<Sensor> UpdateAsync(Guid id, Sensor dados, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
