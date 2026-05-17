using SolarMetrics.Infrastructure.Persistence.Entitites;
using SolarMetrics.Infrastructure.Persistence.Queries;

namespace SolarMetrics.Infrastructure.Persistence.Repositories;

public interface IMonitoramentoRepository
{
    Task<(List<Monitoramento> Items, int TotalCount)> GetPagedAsync(MonitoramentoListQuery query, CancellationToken cancellationToken = default);
    Task<Monitoramento?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Monitoramento> AddAsync(Monitoramento monitoramento, CancellationToken cancellationToken = default);
    Task DeleteAsync(Monitoramento monitoramento, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
