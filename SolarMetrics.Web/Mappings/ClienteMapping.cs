using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolarMetrics.Web.Models;

namespace SolarMetrics.Web.Mappings;

public class ClienteMapping : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> b)
    {
        b.ToTable("SM_USUARIO");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .ValueGeneratedNever()
            .HasColumnName("ID")
            .AsOracleUuid36();

        b.Property(x => x.Nome)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("NOME");

        b.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("EMAIL");

        b.HasIndex(x => x.Email)
            .IsUnique();

        b.Property(x => x.TipoUsuario)
            .HasMaxLength(50)
            .HasColumnName("TIPO_USER");

        b.Property(x => x.Telefone)
            .HasMaxLength(30)
            .HasColumnName("TELEFONE");

        b.Property(x => x.UsuarioUsername)
            .HasMaxLength(200)
            .HasColumnName("USUARIO_USERNAME");

        b.HasOne(x => x.Login)
            .WithOne(x => x.Cliente)
            .HasForeignKey<Cliente>(x => x.UsuarioUsername)
            .HasPrincipalKey<Login>(x => x.Username)
            .IsRequired(false);
    }
}
