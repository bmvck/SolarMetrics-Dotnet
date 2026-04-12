using SolarMetrics.Web.Models;

namespace SolarMetrics.Web.Repositories;

public interface IPainelSolarRepository
{
    Task<List<PainelSolar>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PainelSolar?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PainelSolar> AddAsync(PainelSolar painel, CancellationToken cancellationToken = default);
    Task DeleteAsync(PainelSolar painel, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
