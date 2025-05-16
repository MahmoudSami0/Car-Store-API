using CarStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarStore.InfraStructure.Data.Config
{
    public class CarFeaturesConfig : IEntityTypeConfiguration<CarFeatures>
    {
        public void Configure(EntityTypeBuilder<CarFeatures> builder)
        {
            builder.HasKey(cf => new { cf.CarModelId, cf.FeatureId });
            builder.HasOne(cf => cf.CarModel).WithMany(cm => cm.CarFeatures).HasForeignKey(cf => cf.CarModelId);
            builder.HasOne(cf => cf.Feature).WithMany(f => f.CarFeatures).HasForeignKey(cf => cf.FeatureId);


            builder.ToTable("CarFeatures");
        }
    }
}
