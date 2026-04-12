using System.ComponentModel.DataAnnotations;

namespace SolarMetrics.Web.Areas.Admin.ViewModels;

public sealed class SensorFormViewModel
{
    public Guid? Id { get; init; }

    [Required]
    [StringLength(50)]
    public string Tipo { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Ativo";

    [StringLength(200)]
    public string? Localizacao { get; set; }

    [Required]
    public Guid SistemaId { get; set; }
}
