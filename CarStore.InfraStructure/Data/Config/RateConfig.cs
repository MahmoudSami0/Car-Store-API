using CarStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarStore.InfraStructure.Data.Config
{
    public class RateConfig : IEntityTypeConfiguration<Rate>
    {
        public void Configure(EntityTypeBuilder<Rate> builder)
        {
            builder.HasKey(r => new {r.UserId, r.CarModelId});

            builder.HasOne(r => r.User).WithMany(u => u.Rates).HasForeignKey(r => r.UserId);
            builder.HasOne(r => r.CarModel).WithMany(cm => cm.Rates).HasForeignKey(r => r.CarModelId);


            builder.ToTable(t => t.HasCheckConstraint("CK_Rate_Value", "[RateValue] >= 0 AND [RateValue] <= 5"));
            builder.ToTable("Rates");
        }
    }
}
