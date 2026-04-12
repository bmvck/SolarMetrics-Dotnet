using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SolarMetrics.Infrastructure.Persistence.Mappings;

internal static class GuidVarchar36Extensions
{
    /// <summary>Alinha PK/FK ao DDL Oracle VARCHAR2(36) (UUID textual).</summary>
    public static PropertyBuilder<Guid> AsOracleUuid36(this PropertyBuilder<Guid> property) =>
        property
            .HasMaxLength(36)
            .HasConversion(
                v => v.ToString("D"),
                v => Guid.Parse(v));
}
