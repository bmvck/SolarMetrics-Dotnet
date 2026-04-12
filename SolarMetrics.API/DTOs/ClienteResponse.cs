using SolarMetrics.Infrastructure.Persistence.Entitites;

namespace SolarMetrics.DTOs;

public class ClienteResponse
{
    public string Nome { get; set; }
    public string TipoUsuario { get; set; }
    
    
    public static ClienteResponse ToResponse(Cliente cliente)
    {
        return new ClienteResponse
        {
            Nome = cliente.Nome,
            TipoUsuario = cliente.TipoUsuario ?? string.Empty
        };
    }
}