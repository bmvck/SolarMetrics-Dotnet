using SolarMetrics.Web.Models;

namespace SolarMetrics.Web.UseCase;

public interface ISistemaUseCase
{
    Task<List<Sistema>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Sistema> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Sistema> CreateAsync(Sistema sistema, CancellationToken cancellationToken = default);
    Task<Sistema> UpdateAsync(Guid id, Sistema dados, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
