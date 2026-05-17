using SolarMetrics.Infrastructure.Persistence.Entitites;

namespace SolarMetrics.DTOs;

public class PainelSolarResponse
{
    public Guid Id { get; set; }
    public string Modelo { get; set; } = null!;
    public string Fabricante { get; set; } = null!;
    public int PotenciaMaxima { get; set; }
    public DateTime DataFabricacao { get; set; }
    public int Eficiencia { get; set; }
    public Guid SistemaId { get; set; }

    public static PainelSolarResponse ToResponse(PainelSolar painel) => new()
    {
        Id = painel.Id,
        Modelo = painel.Modelo,
        Fabricante = painel.Fabricante,
        PotenciaMaxima = painel.PotenciaMaxima,
        DataFabricacao = painel.DataFabricacao,
        Eficiencia = painel.Eficiencia,
        SistemaId = painel.SistemaId
    };
}
