using SolarMetrics.Common;

namespace SolarMetrics.Infrastructure.Persistence.Queries;

public sealed class SistemaListQuery : PagedQuery
{
    public string? NomeInstalacao { get; set; }
    public string? Status { get; set; }
    public Guid? ClienteId { get; set; }
}
