using SolarMetrics.Infrastructure.Persistence.Entitites;
using SolarMetrics.Infrastructure.Persistence.Queries;

namespace SolarMetrics.UseCase;

public interface ISistemaUseCase
{
    Task<(List<Sistema> Items, int TotalCount)> GetPagedAsync(SistemaListQuery query, CancellationToken cancellationToken = default);
    Task<Sistema> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Sistema> CreateAsync(Sistema sistema, CancellationToken cancellationToken = default);
    Task<Sistema> UpdateAsync(Guid id, Sistema dados, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
