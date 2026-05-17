using Microsoft.EntityFrameworkCore;
using SolarMetrics.Domain;
using SolarMetrics.Exceptions;
using SolarMetrics.Infrastructure.Persistence.Entitites;
using SolarMetrics.Infrastructure.Persistence.Queries;
using SolarMetrics.Infrastructure.Persistence.Repositories;

namespace SolarMetrics.UseCase;

public class ClienteUseCase(IClienteRepository clienteRepository) : IClienteUseCase
{
    public async Task<Cliente> CreateAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        ValidarEmail(cliente.Email);
        var existe = await clienteRepository.FindEmailAsync(cliente.Email, cancellationToken: cancellationToken);
        if (existe != null)
            throw new EmailDuplicadoException();
        try
        {
            return await clienteRepository.AddAsync(cliente, cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
        {
            throw new EmailDuplicadoException();
        }
    }

    public async Task<Cliente> UpdateAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        ValidarEmail(cliente.Email);
        var clienteExistente = await GetById(cliente.Id, cancellationToken);
        var result = await clienteRepository.FindEmailAsync(cliente.Email, cliente.Id, cancellationToken);
        if (result != null)
            throw new EmailDuplicadoException();

        clienteExistente.Nome = cliente.Nome;
        clienteExistente.Email = cliente.Email;
        clienteExistente.Telefone = cliente.Telefone;
        clienteExistente.TipoUsuario = cliente.TipoUsuario;

        try
        {
            await clienteRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
        {
            throw new EmailDuplicadoException();
        }

        return clienteExistente;
    }

    public async Task<Cliente> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var cliente = await clienteRepository.GetByIdAsync(id, cancellationToken);
        if (cliente == null)
            throw new ClienteNaoEncontradoException();
        return cliente;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cliente = await GetById(id, cancellationToken);
        await clienteRepository.DeleteAsync(cliente, cancellationToken);
    }

    public Task<(List<Cliente> Items, int TotalCount)> GetPagedAsync(
        ClienteListQuery query,
        CancellationToken cancellationToken = default) =>
        clienteRepository.GetPagedAsync(query, cancellationToken);

    private static void ValidarEmail(string email)
    {
        if (!EmailFormatoRegra.EhValido(email))
            throw new ArgumentException("E-mail inválido.");
    }
}
