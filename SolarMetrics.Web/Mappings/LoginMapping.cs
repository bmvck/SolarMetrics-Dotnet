using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolarMetrics.Web.Models;

namespace SolarMetrics.Web.Mappings;

public class LoginMapping : IEntityTypeConfiguration<Login>
{
    public void Configure(EntityTypeBuilder<Login> b)
    {
        b.ToTable("SM_LOGIN");

        b.HasKey(x => x.Username);

        b.Property(x => x.Username)
            .HasMaxLength(200)
            .HasColumnName("USERNAME");

        b.Property(x => x.Password)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("PASSWORD");

        b.HasMany(x => x.Roles)
            .WithOne(x => x.Login)
            .HasForeignKey(x => x.SmLoginUsername)
            .HasPrincipalKey(x => x.Username)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
