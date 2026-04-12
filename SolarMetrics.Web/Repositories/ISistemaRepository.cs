using SolarMetrics.Web.Models;

namespace SolarMetrics.Web.Repositories;

public interface ISistemaRepository
{
    Task<List<Sistema>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Sistema?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Sistema> AddAsync(Sistema sistema, CancellationToken cancellationToken = default);
    Task DeleteAsync(Sistema sistema, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
