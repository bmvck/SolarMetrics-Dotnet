using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SolarMetrics.Web.Models;

namespace SolarMetrics.Web.Mappings;

public class LoginRoleMapping : IEntityTypeConfiguration<LoginRole>
{
    public void Configure(EntityTypeBuilder<LoginRole> b)
    {
        b.ToTable("SM_LOGIN_ROLES");

        b.HasKey(x => new { x.SmLoginUsername, x.Role });

        b.Property(x => x.SmLoginUsername)
            .HasMaxLength(200)
            .HasColumnName("SM_LOGIN_USERNAME");

        b.Property(x => x.Role)
            .IsRequired()
            .HasMaxLength(100)
            .HasColumnName("ROLES");
    }
}
