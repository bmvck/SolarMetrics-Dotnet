using SolarMetrics.Exceptions;
using SolarMetrics.Infrastructure.Persistence.Entitites;
using SolarMetrics.Infrastructure.Persistence.Queries;
using SolarMetrics.Infrastructure.Persistence.Repositories;

namespace SolarMetrics.UseCase;

public sealed class MonitoramentoUseCase(
    IMonitoramentoRepository monitoramentoRepository,
    ISensorRepository sensorRepository) : IMonitoramentoUseCase
{
    public Task<(List<Monitoramento> Items, int TotalCount)> GetPagedAsync(
        MonitoramentoListQuery query,
        CancellationToken cancellationToken = default) =>
        monitoramentoRepository.GetPagedAsync(query, cancellationToken);

    public async Task<Monitoramento> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var monitoramento = await monitoramentoRepository.GetByIdAsync(id, cancellationToken);
        if (monitoramento == null)
            throw new MonitoramentoNaoEncontradoException();
        return monitoramento;
    }

    public async Task<Monitoramento> CreateAsync(Monitoramento monitoramento, CancellationToken cancellationToken = default)
    {
        if (await sensorRepository.GetByIdAsync(monitoramento.SensorId, cancellationToken) == null)
            throw new SensorNaoEncontradoException();
        return await monitoramentoRepository.AddAsync(monitoramento, cancellationToken);
    }

    public async Task<Monitoramento> UpdateAsync(Guid id, Monitoramento dados, CancellationToken cancellationToken = default)
    {
        var existente = await GetByIdAsync(id, cancellationToken);
        if (await sensorRepository.GetByIdAsync(dados.SensorId, cancellationToken) == null)
            throw new SensorNaoEncontradoException();

        existente.Periodo = dados.Periodo;
        existente.ValorLido = dados.ValorLido;
        existente.MediaLeitura = dados.MediaLeitura;
        existente.MaximaLeitura = dados.MaximaLeitura;
        existente.SensorId = dados.SensorId;

        await monitoramentoRepository.SaveChangesAsync(cancellationToken);
        return existente;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var monitoramento = await GetByIdAsync(id, cancellationToken);
        await monitoramentoRepository.DeleteAsync(monitoramento, cancellationToken);
    }
}
