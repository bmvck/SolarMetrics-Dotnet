using System.ComponentModel.DataAnnotations;
using SolarMetrics.Infrastructure.Persistence.Entitites;

namespace SolarMetrics.DTOs;

public class SistemaDTO
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string NomeInstalacao { get; set; } = null!;

    [Required]
    public DateTime DataInstalacao { get; set; }

    [Range(1, int.MaxValue)]
    public int PotenciaTotal { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = null!;

    [Required]
    public Guid ClienteId { get; set; }

    public static Sistema ToEntity(SistemaDTO dto) => new()
    {
        NomeInstalacao = dto.NomeInstalacao,
        DataInstalacao = dto.DataInstalacao,
        PotenciaTotal = dto.PotenciaTotal,
        Status = dto.Status,
        ClienteId = dto.ClienteId
    };
}
