using CarStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarStore.InfraStructure.Data.Config
{
    public class ModelGalleryConfig : IEntityTypeConfiguration<ModelGallery>
    {
        public void Configure(EntityTypeBuilder<ModelGallery> builder)
        {
            builder.HasKey(mg => mg.Id);
            builder.Property(mg => mg.Id).ValueGeneratedOnAdd();
            builder.Property(mg => mg.ImageUrl).HasMaxLength(256);

            builder.HasOne(mg => mg.CarModel).WithMany(cm => cm.ModelGalleries).HasForeignKey(mg => mg.CarModelId);

            builder.ToTable("ModelGalleries");
        }
    }
}
