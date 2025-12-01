using MarketplaceSystem.Application.Common.Interfaces.IServices;
using MarketplaceSystem.Domain.Entities.Base;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Entities.Identity_Auth;
using MarketplaceSystem.Domain.Entities.System_Log;
using MarketplaceSystem.Infrastructure.Data.Repositories.BusinessRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MarketplaceSystem.Infrastructure.Data.Contexts
{
    public class MarketplaceSystemDbContext : IdentityDbContext<User, Role, int>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<MarketplaceSystemDbContext> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MarketplaceSystemDbContext(
            DbContextOptions<MarketplaceSystemDbContext> options,
            ILogger<MarketplaceSystemDbContext> logger,
            ICurrentUserService currentUser,
            IHttpContextAccessor httpContext) : base(options)
        {
            _currentUser = currentUser;
            _logger = logger;
            _httpContextAccessor = httpContext;
        }

        #region DbSet Section
        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryAttribute> CategoriesAttributes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductClassification> ProductClassifications { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductAttributeValue> ProductAttributeValue { get; set; }
        public DbSet<AttributeOption> AttributeOptions { get; set; }
        public DbSet<FavoriteProduct> FavoriteProducts { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(MarketplaceSystemDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellation = default)
        {
            int? userId = _currentUser.UserId;

            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (
                    e.State == EntityState.Added
                    || e.State == EntityState.Modified
                    || e.State == EntityState.Deleted))
                .ToList();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added when entry.Entity is BaseEntity added:
                        HandleAdded(added, userId);
                        break;

                    case EntityState.Modified when entry.Entity is BaseEntity modified:
                        HandleModified(modified, userId);
                        break;

                    case EntityState.Deleted when entry.Entity is SoftDeleteEntity softDeleted:
                        entry.State = EntityState.Modified;
                        HandleSoftDeleted(softDeleted, userId);
                        break;

                    case EntityState.Deleted when entry.Entity is BaseEntity hardDeleted:
                        HandleHardDeleted(hardDeleted);
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellation);
        }

        private void HandleAdded(BaseEntity entity, int? userId)
        {
            entity.SetCreated(userId);
            _logger.LogInformation("User {User} created entity {EntityType} with Id {EntityId}",
                           _currentUser.UserName, entity.GetType().Name, entity.Id);

            AddAuditLog("Created", entity);
        }

        private void HandleModified(BaseEntity entity, int? userId)
        {
            entity.SetUpdated(userId);
            _logger.LogInformation("User {User} updated entity {EntityType} with Id {EntityId}",
                           _currentUser.UserName, entity.GetType().Name, entity.Id);

            AddAuditLog("Updated", entity);
        }

        private void HandleSoftDeleted(SoftDeleteEntity entity, int? userId)
        {
            entity.SetDeleted(userId);
            _logger.LogWarning("User {User} soft-deleted entity {EntityType} with Id {EntityId}",
                       _currentUser.UserName, entity.GetType().Name, entity.Id);

            AddAuditLog("SoftDeleted", entity);
        }

        private void HandleHardDeleted(BaseEntity entity)
        {
            _logger.LogWarning("User {User} hard-deleted entity {EntityType} with Id {EntityId}",
                       _currentUser.UserName, entity.GetType().Name, entity.Id);

            AddAuditLog("HardDeleted", entity);
        }

        private void AddAuditLog(string action, BaseEntity entity)
        {
            AuditLogs.Add(new AuditLog
            {
                UserId = _currentUser.UserId,
                UserName = _currentUser.UserName,
                Action = action,
                EntityType = entity.GetType().Name,
                EntityId = entity.Id,
                IPAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                UserAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString(),
                CorrelationId = _httpContextAccessor.HttpContext?.TraceIdentifier,
                Source = "EFCore",
                RequestPath = _httpContextAccessor.HttpContext?.Request?.Path,
            });
        }
    }
}
