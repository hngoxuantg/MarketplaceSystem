using MarketplaceSystem.Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketplaceSystem.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationProductAttributeValue : IEntityTypeConfiguration<ProductAttributeValue>
    {
        public void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
        {
            builder.ToTable("ProductAttributeValue");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.TextValue)
                .HasMaxLength(100);

            builder.Property(p => p.SelectValues)
                .HasMaxLength(100);

            builder.HasOne(p => p.Product)
                .WithMany(p => p.ProductAttributeValues)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.CategoryAttribute)
                .WithMany()
                .HasForeignKey(p => p.CategoryAttributeId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
