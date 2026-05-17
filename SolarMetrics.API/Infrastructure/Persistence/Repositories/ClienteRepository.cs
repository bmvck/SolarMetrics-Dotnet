using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SolarMetrics.Common;
using SolarMetrics.Infrastructure.Persistence.Entitites;
using SolarMetrics.Infrastructure.Persistence.Queries;

namespace SolarMetrics.Infrastructure.Persistence.Repositories;

public class ClienteRepository(SolarMetricsContext context) : IClienteRepository
{
    private static readonly Dictionary<string, Expression<Func<Cliente, object>>> SortMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["nome"] = c => c.Nome,
        ["email"] = c => c.Email,
        ["tipousuario"] = c => c.TipoUsuario ?? string.Empty
    };

    public async Task<Cliente> AddAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        context.Clientes.Add(cliente);
        await context.SaveChangesAsync(cancellationToken);
        return cliente;
    }

    public async Task<Cliente?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Clientes.FindAsync([id], cancellationToken);

    public async Task DeleteAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        context.Clientes.Remove(cliente);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<string?> FindEmailAsync(string email, Guid? idToIgnore = null, CancellationToken cancellationToken = default)
    {
        var cliente = await context.Clientes
            .FirstOrDefaultAsync(c => c.Email == email && (idToIgnore == null || c.Id != idToIgnore.Value), cancellationToken);
        return cliente?.Email;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);

    public async Task<(List<Cliente> Items, int TotalCount)> GetPagedAsync(
        ClienteListQuery query,
        CancellationToken cancellationToken = default)
    {
        query.Normalize();
        var q = context.Clientes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Nome))
            q = q.Where(c => c.Nome.Contains(query.Nome));
        if (!string.IsNullOrWhiteSpace(query.Email))
            q = q.Where(c => c.Email.Contains(query.Email));
        if (!string.IsNullOrWhiteSpace(query.TipoUsuario))
            q = q.Where(c => c.TipoUsuario == query.TipoUsuario);

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .ApplySort(query.SortBy, query.SortDir, SortMap, c => c.Nome)
            .Skip(query.Skip)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}
