using SolarMetrics.Exceptions;
using SolarMetrics.Infrastructure.Persistence.Entitites;
using SolarMetrics.Infrastructure.Persistence.Queries;
using SolarMetrics.Infrastructure.Persistence.Repositories;

namespace SolarMetrics.UseCase;

public sealed class PainelSolarUseCase(IPainelSolarRepository painelRepository, ISistemaRepository sistemaRepository) : IPainelSolarUseCase
{
    public Task<(List<PainelSolar> Items, int TotalCount)> GetPagedAsync(
        PainelSolarListQuery query,
        CancellationToken cancellationToken = default) =>
        painelRepository.GetPagedAsync(query, cancellationToken);

    public async Task<PainelSolar> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var painel = await painelRepository.GetByIdAsync(id, cancellationToken);
        if (painel == null)
            throw new PainelSolarNaoEncontradoException();
        return painel;
    }

    public async Task<PainelSolar> CreateAsync(PainelSolar painel, CancellationToken cancellationToken = default)
    {
        if (await sistemaRepository.GetByIdAsync(painel.SistemaId, cancellationToken) == null)
            throw new SistemaNaoEncontradoException();
        return await painelRepository.AddAsync(painel, cancellationToken);
    }

    public async Task<PainelSolar> UpdateAsync(Guid id, PainelSolar dados, CancellationToken cancellationToken = default)
    {
        var existente = await GetByIdAsync(id, cancellationToken);
        if (await sistemaRepository.GetByIdAsync(dados.SistemaId, cancellationToken) == null)
            throw new SistemaNaoEncontradoException();

        existente.Modelo = dados.Modelo;
        existente.Fabricante = dados.Fabricante;
        existente.PotenciaMaxima = dados.PotenciaMaxima;
        existente.DataFabricacao = dados.DataFabricacao;
        existente.Eficiencia = dados.Eficiencia;
        existente.SistemaId = dados.SistemaId;

        await painelRepository.SaveChangesAsync(cancellationToken);
        return existente;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var painel = await GetByIdAsync(id, cancellationToken);
        await painelRepository.DeleteAsync(painel, cancellationToken);
    }
}
