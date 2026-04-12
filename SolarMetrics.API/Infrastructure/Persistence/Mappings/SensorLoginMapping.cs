using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolarMetrics.Infrastructure.Persistence.Entitites;

namespace SolarMetrics.Infrastructure.Persistence.Mappings;

public class SensorLoginMapping : IEntityTypeConfiguration<SensorLogin>
{
    public void Configure(EntityTypeBuilder<SensorLogin> b)
    {
        b.ToTable("SM_SENSOR_LOGIN");

        b.HasKey(x => x.Username);

        b.Property(x => x.Username)
            .HasMaxLength(200)
            .HasColumnName("USERNAME");

        b.Property(x => x.Password)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("PASSWORD");

        b.Property(x => x.IsSuperuser)
            .HasColumnName("IS_SUPERUSER")
            .HasColumnType("NUMBER(1)")
            .HasConversion<int>(
                v => v ? 1 : 0,
                v => v == 1);
    }
}
