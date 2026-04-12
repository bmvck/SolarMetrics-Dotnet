using System.ComponentModel.DataAnnotations;

namespace SolarMetrics.Web.Areas.Admin.ViewModels;

public sealed class SistemaFormViewModel
{
    public Guid? Id { get; init; }

    [Required(ErrorMessage = "Nome da instalação é obrigatório")]
    [StringLength(200, MinimumLength = 2)]
    public string NomeInstalacao { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    public DateTime DataInstalacao { get; set; } = DateTime.Today;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Informe a potência total")]
    public int PotenciaTotal { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Ativo";

    [Required(ErrorMessage = "Selecione o cliente")]
    public Guid ClienteId { get; set; }
}
