using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Infrastructure.Data.Repositories.BusinessRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketplaceSystem.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationProduct : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Product");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .HasMaxLength(300);

            builder.Property(p => p.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(p => p.Condition)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.HasOne(p => p.Category)
                .WithMany(p => p.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(p => p.Seller)
                .WithMany(p => p.Products)
                .HasForeignKey(p => p.SellerId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(p => p.Quantity)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(p => p.Location)
                .IsRequired()
                .HasConversion<string>();

            builder.HasMany(p => p.ProductImages)
                .WithOne(pi => pi.Product)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.ProductClassification)
                .WithOne(pc => pc.Product)
                .HasForeignKey<ProductClassification>(pc => pc.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
