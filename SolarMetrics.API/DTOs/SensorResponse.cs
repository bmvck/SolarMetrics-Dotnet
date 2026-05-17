using SolarMetrics.Infrastructure.Persistence.Entitites;

namespace SolarMetrics.DTOs;

public class SensorResponse
{
    public Guid Id { get; set; }
    public string Tipo { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? Localizacao { get; set; }
    public Guid SistemaId { get; set; }

    public static SensorResponse ToResponse(Sensor sensor) => new()
    {
        Id = sensor.Id,
        Tipo = sensor.Tipo,
        Status = sensor.Status,
        Localizacao = sensor.Localizacao,
        SistemaId = sensor.SistemaId
    };
}
