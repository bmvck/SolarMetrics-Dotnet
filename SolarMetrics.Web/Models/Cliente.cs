namespace SolarMetrics.Web.Models;

public class Cliente
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = null!;
    public string Nome { get; set; } = null!;
    public string? Telefone { get; set; }
    public string? TipoUsuario { get; set; }

    public string? UsuarioUsername { get; set; }
    public Login? Login { get; set; }

    public List<Sistema> Sistemas { get; set; } = [];
}
