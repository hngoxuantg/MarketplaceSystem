using MarketplaceSystem.Domain.Entities.Identity_Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketplaceSystem.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationRole : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Role");

            builder.HasKey(r => r.Id);
        }
    }
}
