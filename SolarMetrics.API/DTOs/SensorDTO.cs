using System.ComponentModel.DataAnnotations;
using SolarMetrics.Infrastructure.Persistence.Entitites;

namespace SolarMetrics.DTOs;

public class SensorDTO
{
    [Required]
    [StringLength(50)]
    public string Tipo { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = null!;

    [StringLength(200)]
    public string? Localizacao { get; set; }

    [Required]
    public Guid SistemaId { get; set; }

    public static Sensor ToEntity(SensorDTO dto) => new()
    {
        Tipo = dto.Tipo,
        Status = dto.Status,
        Localizacao = dto.Localizacao,
        SistemaId = dto.SistemaId
    };
}
