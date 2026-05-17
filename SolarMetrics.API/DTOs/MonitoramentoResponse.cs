using SolarMetrics.Infrastructure.Persistence.Entitites;

namespace SolarMetrics.DTOs;

public class MonitoramentoResponse
{
    public Guid Id { get; set; }
    public DateTime Periodo { get; set; }
    public int ValorLido { get; set; }
    public int MediaLeitura { get; set; }
    public int MaximaLeitura { get; set; }
    public Guid SensorId { get; set; }

    public static MonitoramentoResponse ToResponse(Monitoramento monitoramento) => new()
    {
        Id = monitoramento.Id,
        Periodo = monitoramento.Periodo,
        ValorLido = monitoramento.ValorLido,
        MediaLeitura = monitoramento.MediaLeitura,
        MaximaLeitura = monitoramento.MaximaLeitura,
        SensorId = monitoramento.SensorId
    };
}
