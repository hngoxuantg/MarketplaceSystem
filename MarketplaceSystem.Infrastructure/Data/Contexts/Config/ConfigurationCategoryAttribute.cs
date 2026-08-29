using MarketplaceSystem.Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketplaceSystem.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationCategoryAttribute : IEntityTypeConfiguration<CategoryAttribute>
    {
        public void Configure(EntityTypeBuilder<CategoryAttribute> builder)
        {
            builder.ToTable("CategoryAttribute");

            builder.HasKey(c => c.Id);

            builder.HasOne(c => c.Category)
                .WithMany(c => c.CategoryAttributes)
                .HasForeignKey(c => c.CategoryId);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.DisplayName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.AttributeType)
                .IsRequired()
                .HasMaxLength(100)
                .HasConversion<string>();

            builder.Property(c => c.Placeholder)
                .HasMaxLength(200);
        }
    }
}
