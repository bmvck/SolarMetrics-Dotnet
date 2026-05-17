using SolarMetrics.Common;

namespace SolarMetrics.Infrastructure.Persistence.Queries;

public sealed class ClienteListQuery : PagedQuery
{
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string? TipoUsuario { get; set; }
}
