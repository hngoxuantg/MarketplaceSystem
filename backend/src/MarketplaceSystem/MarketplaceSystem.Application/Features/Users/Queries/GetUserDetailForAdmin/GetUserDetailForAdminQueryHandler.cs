using MarketplaceSystem.Application.Common.DTOs.Users;
using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Application.Common.Interfaces.IExternalServices.IStorageServices;
using MarketplaceSystem.Domain.Enums.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Users.Queries.GetUserDetailForAdmin
{
    public class GetUserDetailForAdminQueryHandler : IRequestHandler<GetUserDetailForAdminQuery, AdminUserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        public GetUserDetailForAdminQueryHandler(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }
        public async Task<AdminUserDto> Handle(GetUserDetailForAdminQuery request, CancellationToken cancellationToken)
        {
            return await GetUserByIdForAdminAsync(request.Id, cancellationToken);
        }
        private async Task<AdminUserDto> GetUserByIdForAdminAsync(int id, CancellationToken cancellation = default)
        {
            return await _unitOfWork.UserRepository.GetOneUntrackedAsync<AdminUserDto>(
                filter: u => u.Id == id && !u.IsDeleted,
                cancellation: cancellation,
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
                }) ?? throw new NotFoundException("Không tìm thấy người dùng!");
        }
    }
}
