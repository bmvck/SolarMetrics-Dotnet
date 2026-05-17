using SolarMetrics.Common;

namespace SolarMetrics.Infrastructure.Persistence.Queries;

public sealed class MonitoramentoListQuery : PagedQuery
{
    public Guid? SensorId { get; set; }
    public string? Periodo { get; set; }
}
