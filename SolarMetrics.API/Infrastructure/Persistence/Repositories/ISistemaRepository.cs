using SolarMetrics.Infrastructure.Persistence.Entitites;
using SolarMetrics.Infrastructure.Persistence.Queries;

namespace SolarMetrics.Infrastructure.Persistence.Repositories;

public interface ISistemaRepository
{
    Task<(List<Sistema> Items, int TotalCount)> GetPagedAsync(SistemaListQuery query, CancellationToken cancellationToken = default);
    Task<Sistema?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Sistema> AddAsync(Sistema sistema, CancellationToken cancellationToken = default);
    Task DeleteAsync(Sistema sistema, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
