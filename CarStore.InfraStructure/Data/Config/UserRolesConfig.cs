using CarStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarStore.InfraStructure.Data.Config;

public class UserRolesConfig : IEntityTypeConfiguration<UserRoles>
{
    public void Configure(EntityTypeBuilder<UserRoles> builder)
    {
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        builder.HasOne(ur => ur.Role).WithMany(r => r.UserRoles);
        builder.HasOne(ur => ur.User).WithMany(u => u.UserRoles);
        
        builder.ToTable("UserRoles");
    }
}