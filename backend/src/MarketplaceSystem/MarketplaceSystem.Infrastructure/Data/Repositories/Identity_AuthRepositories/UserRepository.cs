using MarketplaceSystem.Domain.Entities.Identity_Auth;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IIdentity_AuthRepositories;
using MarketplaceSystem.Infrastructure.Data.Contexts;
using MarketplaceSystem.Infrastructure.Data.Repositories.BaseRepositories;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceSystem.Infrastructure.Data.Repositories.Identity_AuthRepositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(MarketplaceSystemDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Dictionary<string, int>> GetStatusDistributionAsync(
            DateTime toDate,
            CancellationToken cancellationToken = default)
        {
            var users = await _dbContext.Users
                .Where(u => u.Profile != null && u.Profile.CreatedAt <= toDate)
                .Select(u => new
                {
                    u.EmailConfirmed,
                    IsLocked = u.LockoutEnd != null && u.LockoutEnd > DateTime.UtcNow
                })
                .ToListAsync(cancellationToken);

            return new Dictionary<string, int>
            {
                ["Active"] = users.Count(u => u.EmailConfirmed && !u.IsLocked),
                ["Locked"] = users.Count(u => u.IsLocked),
                ["PendingEmailConfirmation"] = users.Count(u => !u.EmailConfirmed && !u.IsLocked)
            };
        }

        public async Task<List<(int UserId, string UserName, string FullName, string Avatar, int TotalViews)>> GetTopActiveUsersAsync(
            int count,
            CancellationToken cancellationToken = default)
        {
            var topUsers = await _dbContext.Users
                .Where(u => u.Profile != null)
                .OrderByDescending(u => u.Products.Sum(p => p.ViewCount))
                .Take(count)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Profile!.FullName,
                    u.Profile.Avatar,
                    TotalViews = u.Products.Sum(p => p.ViewCount)
                })
                .ToListAsync(cancellationToken);

            return topUsers.Select(u => (
                u.Id,
                u.UserName ?? string.Empty,
                u.FullName,
                u.Avatar ?? string.Empty,
                u.TotalViews
            )).ToList();
        }
    }
}
