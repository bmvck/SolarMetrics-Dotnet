using System.ComponentModel.DataAnnotations;

namespace SolarMetrics.DTOs;

public class SistemaDTOUpdate : SistemaDTO
{
    [Required]
    public Guid Id { get; set; }
}
