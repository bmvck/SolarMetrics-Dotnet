using SolarMetrics.Infrastructure.Persistence.Entitites;

namespace SolarMetrics.DTOs;

public class SistemaResponse
{
    public Guid Id { get; set; }
    public string NomeInstalacao { get; set; } = null!;
    public DateTime DataInstalacao { get; set; }
    public int PotenciaTotal { get; set; }
    public string Status { get; set; } = null!;
    public Guid ClienteId { get; set; }

    public static SistemaResponse ToResponse(Sistema sistema) => new()
    {
        Id = sistema.Id,
        NomeInstalacao = sistema.NomeInstalacao,
        DataInstalacao = sistema.DataInstalacao,
        PotenciaTotal = sistema.PotenciaTotal,
        Status = sistema.Status,
        ClienteId = sistema.ClienteId
    };
}
