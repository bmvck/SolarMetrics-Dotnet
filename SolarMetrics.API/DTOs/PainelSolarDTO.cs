using System.ComponentModel.DataAnnotations;
using SolarMetrics.Infrastructure.Persistence.Entitites;

namespace SolarMetrics.DTOs;

public class PainelSolarDTO
{
    [Required]
    [StringLength(100)]
    public string Modelo { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Fabricante { get; set; } = null!;

    [Range(1, int.MaxValue)]
    public int PotenciaMaxima { get; set; }

    [Required]
    public DateTime DataFabricacao { get; set; }

    [Range(0, 100)]
    public int Eficiencia { get; set; }

    [Required]
    public Guid SistemaId { get; set; }

    public static PainelSolar ToEntity(PainelSolarDTO dto) => new()
    {
        Modelo = dto.Modelo,
        Fabricante = dto.Fabricante,
        PotenciaMaxima = dto.PotenciaMaxima,
        DataFabricacao = dto.DataFabricacao,
        Eficiencia = dto.Eficiencia,
        SistemaId = dto.SistemaId
    };
}
