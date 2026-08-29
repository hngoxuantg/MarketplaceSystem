using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBusinessRepositories;
using MarketplaceSystem.Infrastructure.Data.Contexts;
using MarketplaceSystem.Infrastructure.Data.Repositories.BaseRepositories;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceSystem.Infrastructure.Data.Repositories.BusinessRepositories
{
    public class UserProfileRepository : BaseRepository<UserProfile>, IUserProfileRepository
    {
        public UserProfileRepository(MarketplaceSystemDbContext dbContext) : base(dbContext) { }

        public async Task<Dictionary<DateTime, int>> GetUserCountsByDateRangeAsync(
            DateTime fromDate,
            DateTime toDate,
            bool groupByMonth,
            CancellationToken cancellationToken = default)
        {
            if (groupByMonth)
            {
                var monthlyData = await _dbContext.UserProfiles
                    .Where(u => u.CreatedAt <= toDate)
                    .GroupBy(u => new { u.CreatedAt.Year, u.CreatedAt.Month })
                    .Select(g => new
                    {
                        Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Date)
                    .ToListAsync(cancellationToken);

                var result = new Dictionary<DateTime, int>();
                DateTime currentDate = new DateTime(fromDate.Year, fromDate.Month, 1);
                DateTime endDate = new DateTime(toDate.Year, toDate.Month, 1);
                int cumulativeCount = 0;

                while (currentDate <= endDate)
                {
                    var monthData = monthlyData.FirstOrDefault(x => x.Date == currentDate);
                    if (monthData != null)
                    {
                        cumulativeCount = monthData.Count;
                    }
                    result[currentDate] = cumulativeCount;
                    currentDate = currentDate.AddMonths(1);
                }

                return result;
            }
            else
            {
                var dailyData = await _dbContext.UserProfiles
                    .Where(u => u.CreatedAt >= fromDate.Date && u.CreatedAt <= toDate)
                    .GroupBy(u => u.CreatedAt.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Date)
                    .ToListAsync(cancellationToken);

                var result = new Dictionary<DateTime, int>();
                int baseCount = await _dbContext.UserProfiles.CountAsync(u => u.CreatedAt < fromDate.Date, cancellationToken);
                int cumulativeCount = baseCount;

                for (DateTime date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
                {
                    var dayData = dailyData.FirstOrDefault(x => x.Date == date);
                    if (dayData != null)
                    {
                        cumulativeCount += dayData.Count;
                    }
                    result[date] = cumulativeCount;
                }

                return result;
            }
        }

        public async Task<Dictionary<TEnum, int>> GetDistributionByEnumAsync<TEnum>(
            DateTime toDate,
            Func<UserProfile, TEnum> selector,
            CancellationToken cancellationToken = default) where TEnum : struct, Enum
        {
            var data = await _dbContext.UserProfiles
                .Where(u => u.CreatedAt <= toDate)
                .ToListAsync(cancellationToken);

            return data.GroupBy(selector)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<Dictionary<int, int>> GetAgeDistributionAsync(
            DateTime toDate,
            CancellationToken cancellationToken = default)
        {
            var birthDates = await _dbContext.UserProfiles
                .Where(u => u.CreatedAt <= toDate)
                .Select(u => u.DateOfBirth)
                .ToListAsync(cancellationToken);

            DateTime now = DateTime.UtcNow;
            var ages = birthDates
                .Select(dob => now.Year - dob.Year - (now.DayOfYear < dob.DayOfYear ? 1 : 0))
                .GroupBy(age =>
                    age < 18 ? 0 :
                    age <= 24 ? 1 :
                    age <= 34 ? 2 :
                    age <= 44 ? 3 :
                    age <= 54 ? 4 :
                    age <= 64 ? 5 : 6)
                .ToDictionary(g => g.Key, g => g.Count());

            return ages;
        }
    }
}
