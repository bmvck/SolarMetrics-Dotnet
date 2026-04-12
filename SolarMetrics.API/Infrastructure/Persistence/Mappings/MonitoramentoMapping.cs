using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolarMetrics.Infrastructure.Persistence.Entitites;

namespace SolarMetrics.Infrastructure.Persistence.Mappings;

public class MonitoramentoMapping : IEntityTypeConfiguration<Monitoramento>
{
    public void Configure(EntityTypeBuilder<Monitoramento> b)
    {
        b.ToTable("SM_MONITORAMENTO");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnName("ID")
            .AsOracleUuid36();

        b.Property(x => x.Periodo)
            .IsRequired()
            .HasColumnName("PERIODO")
            .HasColumnType("DATE");

        b.Property(x => x.ValorLido)
            .IsRequired()
            .HasColumnName("VALOR_LIDO");

        b.Property(x => x.MediaLeitura)
            .IsRequired()
            .HasColumnName("MEDIA_LEITURA");

        b.Property(x => x.MaximaLeitura)
            .IsRequired()
            .HasColumnName("MAXIMA_LEITURA");

        b.Property(x => x.SensorId)
            .HasColumnName("SENSOR_ID")
            .AsOracleUuid36();

        b.HasOne(x => x.Sensor)
            .WithMany(x => x.Monitoramentos)
            .HasForeignKey(x => x.SensorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
