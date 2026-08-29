using MarketplaceSystem.Application.Common.DTOs.Statistics.Users;
using MarketplaceSystem.Application.Common.Interfaces.IExternalServices.IStorageServices;
using MarketplaceSystem.Common.Extensions;
using MarketplaceSystem.Domain.Enums.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Statistics.Users.Queries.GetSummary
{
    public class GetSummaryQueryHandler : IRequestHandler<GetSummaryQuery, SummaryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public GetSummaryQueryHandler(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<SummaryDto> Handle(GetSummaryQuery query, CancellationToken cancellationToken = default)
        {
            return await GetSummaryAsync(query.Request, cancellationToken);
        }

        private async Task<SummaryDto> GetSummaryAsync(
            GetSummaryRequest request,
            CancellationToken cancellationToken = default)
        {
            DateTime systemStartDate = await GetSystemStartDateAsync(cancellationToken);

            if (request.From < systemStartDate)
            {
                request.From = systemStartDate;
            }
            if (request.To > DateTime.UtcNow)
            {
                request.To = DateTime.UtcNow;
            }

            SummaryDto summaryDto = new SummaryDto();

            summaryDto.TotalUsers = await GetTotalUsersAsync(request, cancellationToken);
            summaryDto.PeriodStats = await GetPeriodStatsAsync(request, cancellationToken);
            summaryDto.ActiveUsers = await GetActiveUsersAsync(request, cancellationToken);
            summaryDto.ActiveUsersPercentage = CalculateActiveUsersPercentage(summaryDto.ActiveUsers, summaryDto.TotalUsers);
            summaryDto.ActiveUsersPeriodStats = await GetActiveUsersPeriodStatsAsync(request, cancellationToken);
            summaryDto.NewUsersThisMonth = await GetNewUsersThisMonthAsync(request, cancellationToken);
            summaryDto.NewUsersLastMonth = await GetNewUsersLastMonthAsync(request, cancellationToken);
            summaryDto.NewUsersChange = CalculatePercentageChange(summaryDto.NewUsersThisMonth, summaryDto.NewUsersLastMonth);
            summaryDto.Timeline = await GetTimelineStatsAsync(request, cancellationToken);
            summaryDto.StatusDistribution = await GetStatusDistributionAsync(request, cancellationToken);
            summaryDto.TopActiveUsers = await GetTopActiveUsersAsync(cancellationToken);
            summaryDto.GenderDistribution = await GetGenderDistributionAsync(request, cancellationToken);
            summaryDto.AgeDistribution = await GetAgeDistributionAsync(request, cancellationToken);
            summaryDto.LocationDistribution = await GetLocationDistributionAsync(request, cancellationToken);

            return summaryDto;
        }


        private async Task<DateTime> GetSystemStartDateAsync(CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.UserProfileRepository.GetOneUntrackedAsync(
                orderBy: q => q.OrderBy(u => u.CreatedAt),
                selector: u => u.CreatedAt,
                cancellation: cancellationToken
            );
        }

        private async Task<int> GetTotalUsersAsync(GetSummaryRequest request, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.UserProfileRepository.GetCountAsync(
                filter: u => u.CreatedAt <= request.To,
                cancellation: cancellationToken
            );
        }

        private async Task<PeriodStatsDto> GetPeriodStatsAsync(GetSummaryRequest request, CancellationToken cancellationToken = default)
        {
            int startPeriodUsers = await GetStartPeriodUsersAsync(request, cancellationToken);
            int endPeriodUsers = await GetEndPeriodUsersAsync(request, cancellationToken);
            int newUsers = await GetNewUsersInPeriodAsync(request, cancellationToken);

            double growthPercentage = CalculateGrowthPercentage(startPeriodUsers, endPeriodUsers);

            return new PeriodStatsDto
            {
                StartPeriodUsers = startPeriodUsers,
                EndPeriodUsers = endPeriodUsers,
                NewUsers = newUsers,
                GrowthPercentage = growthPercentage
            };
        }

        private double CalculateGrowthPercentage(int startValue, int endValue)
        {
            if (startValue == 0)
            {
                return endValue == 0 ? 0 : 100;
            }

            return ((double)(endValue - startValue) / startValue) * 100;
        }

        private async Task<int> GetStartPeriodUsersAsync(GetSummaryRequest request, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.UserProfileRepository.GetCountAsync(
                filter: u => u.CreatedAt < request.From,
                cancellation: cancellationToken
            );
        }
        private async Task<int> GetEndPeriodUsersAsync(GetSummaryRequest request, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.UserProfileRepository.GetCountAsync(
                filter: u => u.CreatedAt <= request.To,
                cancellation: cancellationToken
            );
        }

        private async Task<int> GetNewUsersInPeriodAsync(GetSummaryRequest request, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.UserProfileRepository.GetCountAsync(
                filter: u => u.CreatedAt >= request.From && u.CreatedAt <= request.To,
                cancellation: cancellationToken
            );
        }

        private async Task<int> GetActiveUsersAsync(GetSummaryRequest request, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.UserRepository.GetCountAsync(
                filter: u => u.LastLoginAt >= request.From && u.LastLoginAt <= request.To,
                cancellation: cancellationToken
            );
        }

        private int CalculateActiveUsersPercentage(int activeUsers, int totalUsers)
        {
            if (totalUsers == 0)
            {
                return 0;
            }

            return (int)((double)activeUsers / totalUsers * 100);
        }

        private async Task<ActiveUsersPeriodStatsDto> GetActiveUsersPeriodStatsAsync(
            GetSummaryRequest request,
            CancellationToken cancellationToken = default)
        {
            int startPeriodActiveUsers = await GetStartPeriodActiveUsersAsync(request, cancellationToken);
            int endPeriodActiveUsers = await GetEndPeriodActiveUsersAsync(request, cancellationToken);
            int newActiveUsers = await GetNewActiveUsersAsync(request, cancellationToken);
            double growthPercentage = CalculateActiveUsersGrowthPercentage(startPeriodActiveUsers, endPeriodActiveUsers);

            return new ActiveUsersPeriodStatsDto
            {
                StartPeriodActiveUsers = startPeriodActiveUsers,
                EndPeriodActiveUsers = endPeriodActiveUsers,
                NewActiveUsers = newActiveUsers,
                ActiveUsersGrowthPercentage = growthPercentage
            };
        }

        private async Task<int> GetStartPeriodActiveUsersAsync(
            GetSummaryRequest request,
            CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.UserRepository.GetCountAsync(
                filter: u => u.LastLoginAt < request.From,
                cancellation: cancellationToken
            );
        }

        private async Task<int> GetEndPeriodActiveUsersAsync(
            GetSummaryRequest request,
            CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.UserRepository.GetCountAsync(
                filter: u => u.LastLoginAt <= request.To,
                cancellation: cancellationToken
            );
        }

        private async Task<int> GetNewActiveUsersAsync(
            GetSummaryRequest request,
            CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.UserRepository.GetCountAsync(
                filter: u => u.LastLoginAt >= request.From && u.LastLoginAt <= request.To,
                cancellation: cancellationToken
            );
        }

        private double CalculateActiveUsersGrowthPercentage(int startValue, int endValue)
        {
            if (startValue == 0)
            {
                return endValue == 0 ? 0 : 100;
            }

            return ((double)(endValue - startValue) / startValue) * 100;
        }

        private async Task<int> GetNewUsersThisMonthAsync(
            GetSummaryRequest request,
            CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.UserProfileRepository.GetCountAsync(
                filter: u => u.CreatedAt.Month == DateTime.UtcNow.Month && u.CreatedAt.Year == DateTime.UtcNow.Year,
                cancellation: cancellationToken
            );
        }

        private async Task<int> GetNewUsersLastMonthAsync(
            GetSummaryRequest request,
            CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.UserProfileRepository.GetCountAsync(
                filter: u => u.CreatedAt.Month == DateTime.UtcNow.AddMonths(-1).Month
                             && u.CreatedAt.Year == DateTime.UtcNow.AddMonths(-1).Year,
                cancellation: cancellationToken
            );
        }

        private double CalculatePercentageChange(int currentValue, int previousValue)
        {
            if (previousValue == 0)
            {
                return currentValue == 0 ? 0 : 100;
            }

            return ((double)(currentValue - previousValue) / previousValue) * 100;
        }

        private async Task<List<TimelineStatsDto>> GetTimelineStatsAsync(
            GetSummaryRequest request,
            CancellationToken cancellationToken = default)
        {
            TimeSpan duration = request.To - request.From;
            bool isMonthly = duration.TotalDays > 60;

            var userCountsByDate = await _unitOfWork.UserProfileRepository.GetUserCountsByDateRangeAsync(
                request.From,
                request.To,
                isMonthly,
                cancellationToken
            );

            var newUserCounts = await _unitOfWork.UserProfileRepository.GetAllAsync(
                filter: u => u.CreatedAt >= request.From && u.CreatedAt <= request.To,
                selector: u => u.CreatedAt,
                cancellation: cancellationToken
            );

            List<TimelineStatsDto> timeline = new List<TimelineStatsDto>();

            foreach (var kvp in userCountsByDate.OrderBy(x => x.Key))
            {
                DateTime periodStart, periodEnd;
                string periodFormat;

                if (isMonthly)
                {
                    periodStart = kvp.Key;
                    periodEnd = kvp.Key.AddMonths(1).AddDays(-1);
                    if (periodEnd > request.To) periodEnd = request.To;
                    periodFormat = kvp.Key.ToString("yyyy-MM");
                }
                else
                {
                    periodStart = kvp.Key.Date;
                    periodEnd = kvp.Key.Date.AddDays(1).AddTicks(-1);
                    if (periodEnd > request.To) periodEnd = request.To;
                    periodFormat = kvp.Key.ToString("yyyy-MM-dd");
                }

                int newUsers = newUserCounts.Count(d => d >= periodStart && d <= periodEnd);

                timeline.Add(new TimelineStatsDto
                {
                    Period = periodFormat,
                    TotalUsers = kvp.Value,
                    NewUsers = newUsers
                });
            }

            return timeline;
        }

        private async Task<UserStatusDistributionDto> GetStatusDistributionAsync(
            GetSummaryRequest request,
            CancellationToken cancellationToken = default)
        {
            var statusData = await _unitOfWork.UserRepository.GetStatusDistributionAsync(
                request.To,
                cancellationToken
            );

            return new UserStatusDistributionDto
            {
                Active = statusData.GetValueOrDefault("Active", 0),
                Locked = statusData.GetValueOrDefault("Locked", 0),
                PendingEmailConfirmation = statusData.GetValueOrDefault("PendingEmailConfirmation", 0)
            };
        }

        private async Task<List<TopActiveUserDto>> GetTopActiveUsersAsync(
            CancellationToken cancellationToken = default)
        {
            var topUsers = await _unitOfWork.UserRepository.GetTopActiveUsersAsync(10, cancellationToken);

            List<TopActiveUserDto> result = new List<TopActiveUserDto>();
            foreach (var user in topUsers)
            {
                string avatarUrl = string.Empty;
                if (!string.IsNullOrEmpty(user.Avatar))
                {
                    avatarUrl = _fileService.GetFileUrl(user.Avatar);
                }

                result.Add(new TopActiveUserDto
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    Avatar = avatarUrl,
                    TotalProductViews = user.TotalViews
                });
            }

            return result;
        }

        private async Task<GenderDistributionDto> GetGenderDistributionAsync(
            GetSummaryRequest request,
            CancellationToken cancellationToken = default)
        {
            var genderData = await _unitOfWork.UserProfileRepository.GetDistributionByEnumAsync(
                request.To,
                u => u.Gender,
                cancellationToken
            );

            return new GenderDistributionDto
            {
                Male = genderData.GetValueOrDefault(UserProfileGender.Male, 0),
                Female = genderData.GetValueOrDefault(UserProfileGender.Female, 0),
                Other = genderData.GetValueOrDefault(UserProfileGender.Other, 0)
            };
        }

        private async Task<AgeDistributionDto> GetAgeDistributionAsync(
            GetSummaryRequest request,
            CancellationToken cancellationToken = default)
        {
            var ageData = await _unitOfWork.UserProfileRepository.GetAgeDistributionAsync(
                request.To,
                cancellationToken
            );

            return new AgeDistributionDto
            {
                Under18 = ageData.GetValueOrDefault(0, 0),
                From18To24 = ageData.GetValueOrDefault(1, 0),
                From25To34 = ageData.GetValueOrDefault(2, 0),
                From35To44 = ageData.GetValueOrDefault(3, 0),
                From45To54 = ageData.GetValueOrDefault(4, 0),
                From55To64 = ageData.GetValueOrDefault(5, 0),
                Above65 = ageData.GetValueOrDefault(6, 0)
            };
        }

        private async Task<LocationDistributionDto> GetLocationDistributionAsync(
            GetSummaryRequest request,
            CancellationToken cancellationToken = default)
        {
            var locationData = await _unitOfWork.UserProfileRepository.GetDistributionByEnumAsync(
                request.To,
                u => u.Location,
                cancellationToken
            );

            int totalUsers = locationData.Values.Sum();
            if (totalUsers == 0)
            {
                return new LocationDistributionDto
                {
                    TopLocations = new List<LocationStatsDto>(),
                    OtherPercentage = 0
                };
            }

            var topLocations = locationData
                .OrderByDescending(x => x.Value)
                .Take(5)
                .Select(x => new LocationStatsDto
                {
                    LocationName = x.Key.GetDisplayName(),
                    UserCount = x.Value,
                    Percentage = (double)x.Value / totalUsers * 100
                })
                .ToList();

            double topPercentage = topLocations.Sum(x => x.Percentage);
            double otherPercentage = 100 - topPercentage;

            return new LocationDistributionDto
            {
                TopLocations = topLocations,
                OtherPercentage = otherPercentage
            };
        }
    }
}
