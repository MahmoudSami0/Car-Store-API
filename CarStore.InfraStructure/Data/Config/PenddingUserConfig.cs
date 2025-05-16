using CarStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarStore.InfraStructure.Data.Config
{
    public class PenddingUserConfig : IEntityTypeConfiguration<PenddingUser>
    {
        public void Configure(EntityTypeBuilder<PenddingUser> builder)
        {
            builder.HasKey(pu => pu.Id);
            builder.Property(pu => pu.Id).ValueGeneratedOnAdd();
            builder.Property(u => u.Name).HasMaxLength(150).IsRequired();
            builder.Property(u => u.Email).HasMaxLength(150).IsRequired();
            builder.Property(u => u.Phone).HasMaxLength(20).IsRequired(false);

            builder.HasIndex(u => u.Email).IsUnique();
            builder.HasIndex(u => u.Phone).IsUnique();

            builder.ToTable("PenddingUsers");
        }
    }
}
