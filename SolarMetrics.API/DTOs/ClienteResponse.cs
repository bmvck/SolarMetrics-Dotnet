using SolarMetrics.Infrastructure.Persistence.Entitites;

namespace SolarMetrics.DTOs;

public class ClienteResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Telefone { get; set; }
    public string TipoUsuario { get; set; } = null!;

    public static ClienteResponse ToResponse(Cliente cliente) => new()
    {
        Id = cliente.Id,
        Nome = cliente.Nome,
        Email = cliente.Email,
        Telefone = cliente.Telefone,
        TipoUsuario = cliente.TipoUsuario ?? string.Empty
    };
}
