using CarStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarStore.InfraStructure.Data.Config
{
    public class CarModelConfig : IEntityTypeConfiguration<CarModel>
    {
        public void Configure(EntityTypeBuilder<CarModel> builder)
        {
            builder.HasKey(m => m.CarId);
            builder.Property(m => m.CarId).ValueGeneratedOnAdd();
            builder.Property(m => m.ModelName).HasMaxLength(500).IsRequired();

            builder.HasOne(m => m.CarBrand).WithMany(b => b.CarModels).HasForeignKey(m => m.CarBrandId);

            builder.HasIndex(m => m.ModelName).IsUnique();

            builder.ToTable("CarModels");
        }
    }
}
