using MarketplaceSystem.Domain.Entities.System_Log;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketplaceSystem.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationAuditLog : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLog");

            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.User)
                .WithMany(au => au.AuditLogs)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
