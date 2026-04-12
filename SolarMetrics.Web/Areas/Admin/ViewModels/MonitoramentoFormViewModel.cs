using System.ComponentModel.DataAnnotations;

namespace SolarMetrics.Web.Areas.Admin.ViewModels;

public sealed class MonitoramentoFormViewModel
{
    public Guid? Id { get; init; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime Periodo { get; set; } = DateTime.Today;

    [Required]
    public int ValorLido { get; set; }

    [Required]
    public int MediaLeitura { get; set; }

    [Required]
    public int MaximaLeitura { get; set; }

    [Required]
    public Guid SensorId { get; set; }
}
