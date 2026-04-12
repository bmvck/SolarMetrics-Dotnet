using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolarMetrics.Infrastructure.Persistence.Entitites;

namespace SolarMetrics.Infrastructure.Persistence.Mappings;

public class AuditoriaMapping : IEntityTypeConfiguration<Auditoria>
{
    public void Configure(EntityTypeBuilder<Auditoria> b)
    {
        b.ToTable("SM_AUDITORIA");

        b.HasKey(x => x.IdAuditoria);

        b.Property(x => x.IdAuditoria)
            .HasColumnName("ID_AUDITORIA");

        b.Property(x => x.NomeTabela)
            .IsRequired()
            .HasMaxLength(128)
            .HasColumnName("NOME_TABELA");

        b.Property(x => x.Operacao)
            .IsRequired()
            .HasMaxLength(10)
            .HasColumnName("OPERACAO");

        b.Property(x => x.UsuarioOracle)
            .IsRequired()
            .HasMaxLength(128)
            .HasColumnName("USUARIO_ORACLE");

        b.Property(x => x.DataOperacao)
            .IsRequired()
            .HasColumnName("DATA_OPERACAO");

        b.Property(x => x.DadosOld)
            .HasColumnName("DADOS_OLD")
            .HasColumnType("CLOB");

        b.Property(x => x.DadosNew)
            .HasColumnName("DADOS_NEW")
            .HasColumnType("CLOB");
    }
}
