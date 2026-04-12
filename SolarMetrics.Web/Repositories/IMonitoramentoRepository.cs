using SolarMetrics.Web.Models;

namespace SolarMetrics.Web.Repositories;

public interface IMonitoramentoRepository
{
    Task<List<Monitoramento>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Monitoramento?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Monitoramento> AddAsync(Monitoramento monitoramento, CancellationToken cancellationToken = default);
    Task DeleteAsync(Monitoramento monitoramento, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
