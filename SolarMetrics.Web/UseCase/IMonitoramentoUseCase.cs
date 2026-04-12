using SolarMetrics.Web.Models;

namespace SolarMetrics.Web.UseCase;

public interface IMonitoramentoUseCase
{
    Task<List<Monitoramento>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Monitoramento> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Monitoramento> CreateAsync(Monitoramento monitoramento, CancellationToken cancellationToken = default);
    Task<Monitoramento> UpdateAsync(Guid id, Monitoramento dados, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
