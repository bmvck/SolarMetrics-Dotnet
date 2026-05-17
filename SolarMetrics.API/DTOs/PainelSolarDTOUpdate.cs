using System.ComponentModel.DataAnnotations;

namespace SolarMetrics.DTOs;

public class PainelSolarDTOUpdate : PainelSolarDTO
{
    [Required]
    public Guid Id { get; set; }
}
