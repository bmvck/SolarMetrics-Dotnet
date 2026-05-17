using SolarMetrics.Common;

namespace SolarMetrics.Infrastructure.Persistence.Queries;

public sealed class SensorListQuery : PagedQuery
{
    public string? Tipo { get; set; }
    public string? Status { get; set; }
    public Guid? SistemaId { get; set; }
}
