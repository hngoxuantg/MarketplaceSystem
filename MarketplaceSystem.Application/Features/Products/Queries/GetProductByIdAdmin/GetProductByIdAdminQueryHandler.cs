using MarketplaceSystem.Application.Common.DTOs.Products;
using MarketplaceSystem.Application.Common.DTOs.Users;
using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Application.Common.Interfaces.IExternalServices.IStorageServices;
using MarketplaceSystem.Common.Extensions;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Products.Queries.GetProductByIdAdmin
{
    public class GetProductByIdAdminQueryHandler : IRequestHandler<GetProductByIdAdminQuery, ProductAdminCardDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        public GetProductByIdAdminQueryHandler(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _fileService = fileService;
            _unitOfWork = unitOfWork;
        }
        public async Task<ProductAdminCardDto> Handle(GetProductByIdAdminQuery request, CancellationToken cancellationToken)
        {
            return await GetProductByIdAsync(request.Id, cancellationToken);
        }
        private async Task<ProductAdminCardDto> GetProductByIdAsync(int productId, CancellationToken cancellation = default)
        {
            return await _unitOfWork.ProductRepository.GetOneUntrackedAsync(
                filter: p => p.Id == productId && !p.IsDeleted,
                selector: p => new ProductAdminCardDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ProductImages.Any(pi => pi.IsMain)
                    ? _fileService.GetAbsoluteUrl(p.ProductImages.First(pi => pi.IsMain).ImageUrl)
                    : p.ProductImages.Any()
                        ? _fileService.GetAbsoluteUrl(p.ProductImages.First().ImageUrl)
                        : null,
                    Condition = p.Condition.GetDisplayName(),
                    Status = p.Status.GetDisplayName(),
                    Location = p.Location.GetDisplayName(),
                    CategoryName = p.Category != null ? p.Category.Name : "",
                    CreateAt = p.CreatedAt.ToVietnamTime(),
                    Seller = new SellerDto
                    {
                        UserId = p.Seller.Id,
                        DisplayName = p.Seller.Profile != null ? p.Seller.Profile.FullName : p.Seller.UserName,
                        PhoneNumber = p.Seller.PhoneNumber ?? "",
                        AvatarUrl = p.Seller.Profile != null && !string.IsNullOrEmpty(p.Seller.Profile.Avatar)
                        ? _fileService.GetAbsoluteUrl(p.Seller.Profile.Avatar)
                        : ""
                    },
                    Flag = p.ProductClassification.WarningFlag,
                    WarningDetail = p.ProductClassification.WarningDetail
                }) ?? throw new NotFoundException("Không tìm thấy sản phẩm!");
        }
    }
}
