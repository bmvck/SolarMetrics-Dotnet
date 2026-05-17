using System.ComponentModel.DataAnnotations;

namespace SolarMetrics.DTOs;

public class SensorDTOUpdate : SensorDTO
{
    [Required]
    public Guid Id { get; set; }
}
