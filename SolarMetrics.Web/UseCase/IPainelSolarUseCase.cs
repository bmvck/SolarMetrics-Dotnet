using SolarMetrics.Web.Models;

namespace SolarMetrics.Web.UseCase;

public interface IPainelSolarUseCase
{
    Task<List<PainelSolar>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PainelSolar> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PainelSolar> CreateAsync(PainelSolar painel, CancellationToken cancellationToken = default);
    Task<PainelSolar> UpdateAsync(Guid id, PainelSolar dados, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
