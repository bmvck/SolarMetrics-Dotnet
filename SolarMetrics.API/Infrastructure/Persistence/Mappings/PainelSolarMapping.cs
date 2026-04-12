using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolarMetrics.Infrastructure.Persistence.Entitites;

namespace SolarMetrics.Infrastructure.Persistence.Mappings;

public class PainelSolarMapping : IEntityTypeConfiguration<PainelSolar>
{
    public void Configure(EntityTypeBuilder<PainelSolar> b)
    {
        b.ToTable("SM_PAINEL_SOLAR");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnName("ID")
            .AsOracleUuid36();

        b.Property(x => x.Modelo)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("MODELO");

        b.Property(x => x.Fabricante)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("FABRICANTE");

        b.Property(x => x.PotenciaMaxima)
            .IsRequired()
            .HasColumnName("POTENCIA_MAXIMA");

        b.Property(x => x.DataFabricacao)
            .IsRequired()
            .HasColumnName("DATA_FABRICACAO")
            .HasColumnType("DATE");

        b.Property(x => x.Eficiencia)
            .IsRequired()
            .HasColumnName("EFICIENCIA");

        b.Property(x => x.SistemaId)
            .HasColumnName("SISTEMA_ID")
            .AsOracleUuid36();

        b.HasOne(x => x.Sistema)
            .WithMany(x => x.PaineisSolares)
            .HasForeignKey(x => x.SistemaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
