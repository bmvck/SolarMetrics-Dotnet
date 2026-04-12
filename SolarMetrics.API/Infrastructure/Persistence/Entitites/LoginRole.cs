namespace SolarMetrics.Infrastructure.Persistence.Entitites;

public class LoginRole
{
    public string SmLoginUsername { get; set; } = null!;
    public string Role { get; set; } = null!;

    public Login Login { get; set; } = null!;
}
