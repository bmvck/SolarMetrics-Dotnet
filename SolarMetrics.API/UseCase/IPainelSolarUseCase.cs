using SolarMetrics.Infrastructure.Persistence.Entitites;
using SolarMetrics.Infrastructure.Persistence.Queries;

namespace SolarMetrics.UseCase;

public interface IPainelSolarUseCase
{
    Task<(List<PainelSolar> Items, int TotalCount)> GetPagedAsync(PainelSolarListQuery query, CancellationToken cancellationToken = default);
    Task<PainelSolar> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PainelSolar> CreateAsync(PainelSolar painel, CancellationToken cancellationToken = default);
    Task<PainelSolar> UpdateAsync(Guid id, PainelSolar dados, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
