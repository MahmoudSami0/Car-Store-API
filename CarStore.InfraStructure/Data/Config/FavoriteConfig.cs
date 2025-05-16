using CarStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarStore.InfraStructure.Data.Config
{
    public class FavoriteConfig : IEntityTypeConfiguration<Favorite>
    {
        public void Configure(EntityTypeBuilder<Favorite> builder)
        {
            builder.HasKey(f => new { f.UserId, f.CarId });

            builder.HasOne(f => f.User).WithMany(u => u.Favorites).HasForeignKey(f => f.UserId);
            builder.HasOne(f => f.CarModel).WithMany(c => c.Favorites).HasForeignKey(f => f.CarId);

            builder.ToTable("Favorites");
        }
    }
}
