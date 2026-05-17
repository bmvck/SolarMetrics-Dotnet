using SolarMetrics.Infrastructure.Persistence.Entitites;
using SolarMetrics.Infrastructure.Persistence.Queries;

namespace SolarMetrics.UseCase;

public interface IClienteUseCase
{
    Task<Cliente> CreateAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task<Cliente> UpdateAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task<Cliente> GetById(Guid id, CancellationToken cancellationToken = default);
    Task<(List<Cliente> Items, int TotalCount)> GetPagedAsync(ClienteListQuery query, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
