using MarketplaceSystem.Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketplaceSystem.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationAttributeOption : IEntityTypeConfiguration<AttributeOption>
    {
        public void Configure(EntityTypeBuilder<AttributeOption> builder)
        {
            builder.ToTable("AttributeOption");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Value)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.DisplayText)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.IsActive)
                .IsRequired();

            builder.HasOne(a => a.CategoryAttribute)
                .WithMany(ca => ca.AttributeOptions)
                .HasForeignKey(a => a.CategoryAttributeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
