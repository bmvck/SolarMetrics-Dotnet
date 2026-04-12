namespace SolarMetrics.Web.Models;

public class PainelSolar
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Modelo { get; set; } = null!;
    public string Fabricante { get; set; } = null!;
    public int PotenciaMaxima { get; set; }
    public DateTime DataFabricacao { get; set; }
    public int Eficiencia { get; set; }

    public Guid SistemaId { get; set; }
    public Sistema Sistema { get; set; } = null!;
}
