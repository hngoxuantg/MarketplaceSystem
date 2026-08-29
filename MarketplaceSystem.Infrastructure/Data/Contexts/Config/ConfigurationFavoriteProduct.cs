using MarketplaceSystem.Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketplaceSystem.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationFavoriteProduct : IEntityTypeConfiguration<FavoriteProduct>
    {
        public void Configure(EntityTypeBuilder<FavoriteProduct> builder)
        {
            builder.ToTable("FavoriteProduct");

            builder.HasKey(f => new { f.UserId, f.ProductId });

            builder.HasOne(f => f.User)
                .WithMany(f => f.FavoriteProducts)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(f => f.Product)
                .WithMany()
                .HasForeignKey(f => f.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
