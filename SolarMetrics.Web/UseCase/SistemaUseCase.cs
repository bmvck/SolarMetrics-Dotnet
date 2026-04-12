using SolarMetrics.Exceptions;
using SolarMetrics.Web.Models;
using SolarMetrics.Web.Repositories;

namespace SolarMetrics.Web.UseCase;

public sealed class SistemaUseCase(ISistemaRepository sistemaRepository, IClienteRepository clienteRepository) : ISistemaUseCase
{
    public async Task<List<Sistema>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await sistemaRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Sistema> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sistema = await sistemaRepository.GetByIdAsync(id, cancellationToken);
        if (sistema == null)
            throw new SistemaNaoEncontradoException();
        return sistema;
    }

    public async Task<Sistema> CreateAsync(Sistema sistema, CancellationToken cancellationToken = default)
    {
        if (await clienteRepository.GetByIdAsync(sistema.ClienteId) == null)
            throw new ClienteNaoEncontradoException();
        return await sistemaRepository.AddAsync(sistema, cancellationToken);
    }

    public async Task<Sistema> UpdateAsync(Guid id, Sistema dados, CancellationToken cancellationToken = default)
    {
        var existente = await GetByIdAsync(id, cancellationToken);
        if (await clienteRepository.GetByIdAsync(dados.ClienteId) == null)
            throw new ClienteNaoEncontradoException();

        existente.NomeInstalacao = dados.NomeInstalacao;
        existente.DataInstalacao = dados.DataInstalacao;
        existente.PotenciaTotal = dados.PotenciaTotal;
        existente.Status = dados.Status;
        existente.ClienteId = dados.ClienteId;

        await sistemaRepository.SaveChangesAsync(cancellationToken);
        return existente;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sistema = await GetByIdAsync(id, cancellationToken);
        await sistemaRepository.DeleteAsync(sistema, cancellationToken);
    }
}
