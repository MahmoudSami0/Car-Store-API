using CarStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarStore.InfraStructure.Data.Config
{
    public class FeatureConfig : IEntityTypeConfiguration<Feature>
    {
        public void Configure(EntityTypeBuilder<Feature> builder)
        {
            builder.HasKey(f => f.FeatureId);
            builder.Property(f => f.FeatureId).ValueGeneratedOnAdd();
            builder.Property(f => f.FeatureName).HasMaxLength(256).IsRequired();
            builder.HasIndex(f => f.FeatureName).IsUnique();

            builder.ToTable("Features");
        }
    }
}
