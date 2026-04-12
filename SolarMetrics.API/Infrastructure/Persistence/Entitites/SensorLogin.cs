namespace SolarMetrics.Infrastructure.Persistence.Entitites;

public class SensorLogin
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool IsSuperuser { get; set; }

    public Sensor? Sensor { get; set; }
}
