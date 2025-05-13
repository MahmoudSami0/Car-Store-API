using CarStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarStore.InfraStructure.Data.Config
{
    public class BlacklistedTokenConfig : IEntityTypeConfiguration<BlacklistedToken>
    {
        public void Configure(EntityTypeBuilder<BlacklistedToken> builder)
        {
            builder.HasKey(bt => bt.Id);
            builder.Property(bt => bt.Id).ValueGeneratedOnAdd();
            builder.Property(bt => bt.Token).IsRequired();

            builder.ToTable("BlacklistedToken");
        }
    }
}
