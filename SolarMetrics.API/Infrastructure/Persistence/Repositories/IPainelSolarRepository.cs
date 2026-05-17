using SolarMetrics.Infrastructure.Persistence.Entitites;
using SolarMetrics.Infrastructure.Persistence.Queries;

namespace SolarMetrics.Infrastructure.Persistence.Repositories;

public interface IPainelSolarRepository
{
    Task<(List<PainelSolar> Items, int TotalCount)> GetPagedAsync(PainelSolarListQuery query, CancellationToken cancellationToken = default);
    Task<PainelSolar?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PainelSolar> AddAsync(PainelSolar painel, CancellationToken cancellationToken = default);
    Task DeleteAsync(PainelSolar painel, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
