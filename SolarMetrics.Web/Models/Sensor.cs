namespace SolarMetrics.Web.Models;

public class Sensor
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Tipo { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? Localizacao { get; set; }

    public Guid SistemaId { get; set; }
    public Sistema Sistema { get; set; } = null!;

    public string? SensorLoginUsername { get; set; }
    public SensorLogin? SensorLogin { get; set; }

    public List<Monitoramento> Monitoramentos { get; set; } = [];
}
