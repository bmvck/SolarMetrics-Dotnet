using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolarMetrics.Web.Models;

namespace SolarMetrics.Web.Mappings;

public class SensorMapping : IEntityTypeConfiguration<Sensor>
{
    public void Configure(EntityTypeBuilder<Sensor> b)
    {
        b.ToTable("SM_SENSOR");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnName("ID")
            .AsOracleUuid36();

        b.Property(x => x.Tipo)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("TIPO");

        b.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("STATUS");

        b.Property(x => x.Localizacao)
            .HasMaxLength(200)
            .HasColumnName("LOCALIZACAO");

        b.Property(x => x.SistemaId)
            .HasColumnName("SISTEMA_ID")
            .AsOracleUuid36();

        b.Property(x => x.SensorLoginUsername)
            .HasMaxLength(200)
            .HasColumnName("SENSOR_LOGIN_USERNAME");

        b.HasOne(x => x.Sistema)
            .WithMany(x => x.Sensores)
            .HasForeignKey(x => x.SistemaId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.SensorLogin)
            .WithOne(x => x.Sensor)
            .HasForeignKey<Sensor>(x => x.SensorLoginUsername)
            .HasPrincipalKey<SensorLogin>(x => x.Username)
            .IsRequired(false);

        b.HasIndex(x => x.SensorLoginUsername)
            .IsUnique();
    }
}
