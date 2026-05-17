using SolarMetrics.Infrastructure.Persistence.Entitites;
using SolarMetrics.Infrastructure.Persistence.Queries;

namespace SolarMetrics.Infrastructure.Persistence.Repositories;

public interface IClienteRepository
{
    Task<Cliente> AddAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task<Cliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<string?> FindEmailAsync(string email, Guid? idToIgnore = null, CancellationToken cancellationToken = default);
    Task<(List<Cliente> Items, int TotalCount)> GetPagedAsync(ClienteListQuery query, CancellationToken cancellationToken = default);
}
