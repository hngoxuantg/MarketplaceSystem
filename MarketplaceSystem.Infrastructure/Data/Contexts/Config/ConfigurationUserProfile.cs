using MarketplaceSystem.Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketplaceSystem.Infrastructure.Data.Contexts.Config
{
    public class ConfigurationUserProfile : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.ToTable("UserProfile");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(u => u.Avatar)
                .HasMaxLength(1000);

            builder.Property(u => u.Bio)
                .HasMaxLength(1000);

            builder.Property(u => u.Gender)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(u => u.Location)
                .IsRequired()
                .HasConversion<string>();

            builder.HasOne(u => u.User)
                .WithOne(u => u.Profile)
                .HasForeignKey<UserProfile>(up => up.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(u => u.SellerVerificationStatus)
                .IsRequired()
                .HasConversion<string>();
        }
    }
}
