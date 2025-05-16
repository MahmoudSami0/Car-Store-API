using CarStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarStore.InfraStructure.Data.Config
{
    public class CarBrandConfig : IEntityTypeConfiguration<CarBrand>
    {
        public void Configure(EntityTypeBuilder<CarBrand> builder)
        {
            builder.HasKey(b => b.BrandId);
            builder.Property(b => b.BrandId).ValueGeneratedOnAdd();
            builder.Property(b => b.BrandName).HasMaxLength(256).IsRequired();
            builder.Property(b => b.Logo).HasMaxLength(256);

            builder.HasIndex(b => b.BrandName).IsUnique();

            builder.ToTable("CarBrands");
        }
    }
}
