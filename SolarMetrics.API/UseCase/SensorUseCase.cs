using SolarMetrics.Exceptions;
using SolarMetrics.Infrastructure.Persistence.Entitites;
using SolarMetrics.Infrastructure.Persistence.Queries;
using SolarMetrics.Infrastructure.Persistence.Repositories;

namespace SolarMetrics.UseCase;

public sealed class SensorUseCase(ISensorRepository sensorRepository, ISistemaRepository sistemaRepository) : ISensorUseCase
{
    public Task<(List<Sensor> Items, int TotalCount)> GetPagedAsync(
        SensorListQuery query,
        CancellationToken cancellationToken = default) =>
        sensorRepository.GetPagedAsync(query, cancellationToken);

    public async Task<Sensor> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sensor = await sensorRepository.GetByIdAsync(id, cancellationToken);
        if (sensor == null)
            throw new SensorNaoEncontradoException();
        return sensor;
    }

    public async Task<Sensor> CreateAsync(Sensor sensor, CancellationToken cancellationToken = default)
    {
        if (await sistemaRepository.GetByIdAsync(sensor.SistemaId, cancellationToken) == null)
            throw new SistemaNaoEncontradoException();
        return await sensorRepository.AddAsync(sensor, cancellationToken);
    }

    public async Task<Sensor> UpdateAsync(Guid id, Sensor dados, CancellationToken cancellationToken = default)
    {
        var existente = await GetByIdAsync(id, cancellationToken);
        if (await sistemaRepository.GetByIdAsync(dados.SistemaId, cancellationToken) == null)
            throw new SistemaNaoEncontradoException();

        existente.Tipo = dados.Tipo;
        existente.Status = dados.Status;
        existente.Localizacao = dados.Localizacao;
        existente.SistemaId = dados.SistemaId;

        await sensorRepository.SaveChangesAsync(cancellationToken);
        return existente;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sensor = await GetByIdAsync(id, cancellationToken);
        await sensorRepository.DeleteAsync(sensor, cancellationToken);
    }
}
