namespace SolarMetrics.Infrastructure.Persistence.Entitites;

public class Sistema
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string NomeInstalacao { get; set; } = null!;
    public DateTime DataInstalacao { get; set; }
    public int PotenciaTotal { get; set; }
    public string Status { get; set; } = null!;

    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    public List<PainelSolar> PaineisSolares { get; set; } = [];
    public List<Sensor> Sensores { get; set; } = [];
}
