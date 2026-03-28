using SolarMetrics.Infrastructure.Persistence.Entitites;

namespace SolarMetrics.Infrastructure.Persistence.Repositories;

public interface IClienteRepository
{
    Task<Cliente> AddAsync(Cliente cliente);
    Task<Cliente?> GetByIdAsync(Guid id);
    Task DeleteAsync(Cliente cliente);
    Task SaveChangesAsync();
    Task<string?> FindEmailAsync(string email, Guid? idToIgnore = null);
    Task<List<Cliente>> GetAllAsync();
}