using MarketplaceSystem.Infrastructure.Data.Repositories.BusinessRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketplaceSystem.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationProductClassification : IEntityTypeConfiguration<ProductClassification>
    {
        public void Configure(EntityTypeBuilder<ProductClassification> builder)
        {
            builder.ToTable("ProductClassification");

            builder.HasKey(pc => pc.Id);

            builder.Property(pc => pc.WarningFlag)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(pc => pc.WarningDetail)
                   .HasMaxLength(1000);

            builder.HasOne(pc => pc.Product)
                   .WithOne(p => p.ProductClassification)
                   .HasForeignKey<ProductClassification>(pc => pc.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
