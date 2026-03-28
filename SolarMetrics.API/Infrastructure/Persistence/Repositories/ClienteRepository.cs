using Microsoft.EntityFrameworkCore;
using SolarMetrics.Infrastructure.Persistence.Entitites;

namespace SolarMetrics.Infrastructure.Persistence.Repositories;

public class ClienteRepository(SolarMetricsContext context) : IClienteRepository
{
    public async Task<Cliente> AddAsync(Cliente cliente)
    {
        context.Clientes.Add(cliente);
        await context.SaveChangesAsync();
        return cliente;
    }
    
    public async Task<Cliente?> GetByIdAsync(Guid id)
    {
        return await context.Clientes.FindAsync(id);
    }
    
    public async Task DeleteAsync(Cliente cliente)
    {
        context.Clientes.Remove(cliente);
        await context.SaveChangesAsync();
    }
    
    public async Task<string?> FindEmailAsync(string email, Guid? idToIgnore = null)
    {
        var cliente = await context.Clientes
            .FirstOrDefaultAsync(c => c.Email == email && (idToIgnore == null || c.Id != idToIgnore.Value));
        return cliente?.Email;
    }
    
    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
    
    public async Task<List<Cliente>> GetAllAsync()
    {
        return await context.Clientes.ToListAsync();
    }
}