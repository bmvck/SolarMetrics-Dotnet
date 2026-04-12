using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolarMetrics.Web.Models;

namespace SolarMetrics.Web.Mappings;

public class SistemaMapping : IEntityTypeConfiguration<Sistema>
{
    public void Configure(EntityTypeBuilder<Sistema> b)
    {
        b.ToTable("SM_SISTEMA");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnName("ID")
            .AsOracleUuid36();

        b.Property(x => x.NomeInstalacao)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("NOME_INSTALACAO");

        b.Property(x => x.DataInstalacao)
            .IsRequired()
            .HasColumnName("DATA_INSTALACAO")
            .HasColumnType("DATE");

        b.Property(x => x.PotenciaTotal)
            .IsRequired()
            .HasColumnName("POTENCIA_TOTAL");

        b.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(20)
            .HasColumnName("STATUS");

        b.Property(x => x.ClienteId)
            .HasColumnName("CLIENTE_ID")
            .AsOracleUuid36();

        b.HasOne(x => x.Cliente)
            .WithMany(x => x.Sistemas)
            .HasForeignKey(x => x.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
