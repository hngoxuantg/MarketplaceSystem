using MarketplaceSystem.Application.Common.DTOs.Users;
using MarketplaceSystem.Application.Common.Interfaces.IExternalServices.IStorageServices;
using MarketplaceSystem.Application.Common.Interfaces.IServices;
using MarketplaceSystem.Common.Models.Pagination;
using MarketplaceSystem.Domain.Entities.Identity_Auth;
using MarketplaceSystem.Domain.Enums.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;
using System.Linq.Expressions;

namespace MarketplaceSystem.Application.Features.Users.Queries.GetUsers
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PaginatedResult<AdminUserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        private readonly ICurrentUserService _currentUserServices;
        public GetUsersQueryHandler(IUnitOfWork unitOfWork, IFileService fileService, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
            _currentUserServices = currentUserService;
        }

        public async Task<PaginatedResult<AdminUserDto>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
        {
            return await GetUsersAsync(query.Request, cancellationToken);
        }

        private async Task<PaginatedResult<AdminUserDto>> GetUsersAsync(GetUsersRequest request, CancellationToken cancellationToken = default)
        {
            (IEnumerable<AdminUserDto> users, int totalCount) = await _unitOfWork.UserRepository.GetPagedAsync<AdminUserDto>(
                filter: GetExpressionFilter(request),
                selector: u => new AdminUserDto
                {
                    Id = u.Id,
                    FullName = u.Profile.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Avatar = u.Profile.Avatar != null ? _fileService.GetAbsoluteUrl(u.Profile.Avatar) : null,
                    PostStats = new UserPostStatsDto
                    {
                        Approved = u.Products.Where(p => p.Status == Domain.Enums.Business.ProductStatus.Active || p.Status == ProductStatus.Sold).Count(),
                        Pending = u.Products.Where(p => p.Status == ProductStatus.PendingApproval).Count(),
                        Rejected = u.Products.Where(p => p.Status == ProductStatus.Rejected).Count()
                    },
                    CreatedAt = u.CreateAt.ToLocalTime(),
                    Status = u.LockoutEnd >= DateTime.UtcNow ? AdminUserStatus.Locked : AdminUserStatus.Active,
                    IsEmailConfirmed = u.EmailConfirmed,
                    LockoutEnd = u.LockoutEnd <= DateTime.UtcNow ? null : u.LockoutEnd.Value.ToLocalTime()
                },
                orderBy: u => u.OrderByDescending(user => user.CreateAt),
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                cancellationToken: cancellationToken
            );

            return new PaginatedResult<AdminUserDto>(users, totalCount, request.PageNumber, request.PageSize);
        }

        private Expression<Func<User, bool>>? GetExpressionFilter(GetUsersRequest request)
        {
            var search = request.Search?.ToLower();
            var status = request.Status;

            Expression<Func<User, bool>>? filter = u =>
                (string.IsNullOrEmpty(request.Search) || u.Profile.FullName.ToLower().Contains(search)) &&
                (!status.HasValue ||
                    (status.Value == UserStatus.Active && (u.LockoutEnd == null || u.LockoutEnd <= DateTime.UtcNow) && u.EmailConfirmed) ||
                    (status.Value == UserStatus.Locked && u.LockoutEnd != null && u.LockoutEnd > DateTime.UtcNow) ||
                    (status.Value == UserStatus.PendingEmailConfirmation && !u.EmailConfirmed)) &&
                (!u.IsDeleted) && (u.Id != _currentUserServices.UserId);

            return filter;
        }
    }
}
