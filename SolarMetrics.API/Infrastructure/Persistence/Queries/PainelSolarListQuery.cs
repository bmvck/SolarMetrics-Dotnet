using SolarMetrics.Common;

namespace SolarMetrics.Infrastructure.Persistence.Queries;

public sealed class PainelSolarListQuery : PagedQuery
{
    public string? Modelo { get; set; }
    public string? Fabricante { get; set; }
    public Guid? SistemaId { get; set; }
}
