using CarStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarStore.InfraStructure.Data.Config;

public class RoleConfig : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.RoleId);
        builder.Property(r => r.RoleId).ValueGeneratedOnAdd();
        builder.Property(r => r.RoleName).HasMaxLength(150).IsRequired();

        builder.HasIndex(r => r.RoleName).IsUnique();

        builder.ToTable("Roles");
    }
}