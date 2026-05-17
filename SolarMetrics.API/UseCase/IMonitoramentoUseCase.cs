using SolarMetrics.Infrastructure.Persistence.Entitites;
using SolarMetrics.Infrastructure.Persistence.Queries;

namespace SolarMetrics.UseCase;

public interface IMonitoramentoUseCase
{
    Task<(List<Monitoramento> Items, int TotalCount)> GetPagedAsync(MonitoramentoListQuery query, CancellationToken cancellationToken = default);
    Task<Monitoramento> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Monitoramento> CreateAsync(Monitoramento monitoramento, CancellationToken cancellationToken = default);
    Task<Monitoramento> UpdateAsync(Guid id, Monitoramento dados, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
