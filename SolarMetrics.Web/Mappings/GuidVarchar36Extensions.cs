using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SolarMetrics.Web.Mappings;

internal static class GuidVarchar36Extensions
{
    public static PropertyBuilder<Guid> AsOracleUuid36(this PropertyBuilder<Guid> property) =>
        property
            .HasMaxLength(36)
            .HasConversion(
                v => v.ToString("D"),
                v => Guid.Parse(v));
}
