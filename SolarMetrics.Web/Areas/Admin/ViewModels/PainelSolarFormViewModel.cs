using System.ComponentModel.DataAnnotations;

namespace SolarMetrics.Web.Areas.Admin.ViewModels;

public sealed class PainelSolarFormViewModel
{
    public Guid? Id { get; init; }

    [Required]
    [StringLength(100)]
    public string Modelo { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Fabricante { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int PotenciaMaxima { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime DataFabricacao { get; set; } = DateTime.Today;

    [Required]
    [Range(0, 100)]
    public int Eficiencia { get; set; }

    [Required]
    public Guid SistemaId { get; set; }
}
