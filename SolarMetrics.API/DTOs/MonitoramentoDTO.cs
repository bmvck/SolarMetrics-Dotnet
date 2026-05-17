using System.ComponentModel.DataAnnotations;
using SolarMetrics.Infrastructure.Persistence.Entitites;

namespace SolarMetrics.DTOs;

public class MonitoramentoDTO
{
    [Required]
    public DateTime Periodo { get; set; }

    [Required]
    public int ValorLido { get; set; }

    [Required]
    public int MediaLeitura { get; set; }

    [Required]
    public int MaximaLeitura { get; set; }

    [Required]
    public Guid SensorId { get; set; }

    public static Monitoramento ToEntity(MonitoramentoDTO dto) => new()
    {
        Periodo = dto.Periodo,
        ValorLido = dto.ValorLido,
        MediaLeitura = dto.MediaLeitura,
        MaximaLeitura = dto.MaximaLeitura,
        SensorId = dto.SensorId
    };
}
