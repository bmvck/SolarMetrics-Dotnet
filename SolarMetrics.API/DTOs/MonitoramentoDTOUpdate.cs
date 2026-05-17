using System.ComponentModel.DataAnnotations;

namespace SolarMetrics.DTOs;

public class MonitoramentoDTOUpdate : MonitoramentoDTO
{
    [Required]
    public Guid Id { get; set; }
}
