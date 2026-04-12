using SolarMetrics.Exceptions;
using SolarMetrics.Web.Models;
using SolarMetrics.Web.Repositories;

namespace SolarMetrics.Web.UseCase;

public sealed class MonitoramentoUseCase(
    IMonitoramentoRepository monitoramentoRepository,
    ISensorRepository sensorRepository) : IMonitoramentoUseCase
{
    public async Task<List<Monitoramento>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await monitoramentoRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Monitoramento> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var m = await monitoramentoRepository.GetByIdAsync(id, cancellationToken);
        if (m == null)
            throw new MonitoramentoNaoEncontradoException();
        return m;
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
        var m = await GetByIdAsync(id, cancellationToken);
        await monitoramentoRepository.DeleteAsync(m, cancellationToken);
    }
}
