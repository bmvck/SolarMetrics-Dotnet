namespace SolarMetrics.Infrastructure.Persistence.Entitites;

public class Login
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;

    public Cliente? Cliente { get; set; }
    public ICollection<LoginRole> Roles { get; set; } = new List<LoginRole>();
}
