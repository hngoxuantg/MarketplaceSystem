using MarketplaceSystem.Application.Common.DTOs.Products;
using MarketplaceSystem.Application.Common.DTOs.Users;
using MarketplaceSystem.Application.Common.Interfaces.IExternalServices.IStorageServices;
using MarketplaceSystem.Common.Extensions;
using MarketplaceSystem.Common.Models.Pagination;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Enums.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MarketplaceSystem.Application.Features.Products.Queries.GetProductsByFilterAdmin
{
    public class GetProductsByFilterAdminQueryHandler : IRequestHandler<GetProductsByFilterAdminQuery, PaginatedResult<ProductAdminCardDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        public GetProductsByFilterAdminQueryHandler(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }
        public async Task<PaginatedResult<ProductAdminCardDto>> Handle(
            GetProductsByFilterAdminQuery query,
            CancellationToken cancellationToken)
        {
            return await GetProductsByFilter(query.Request, cancellationToken);
        }
        private async Task<PaginatedResult<ProductAdminCardDto>> GetProductsByFilter(
            GetProductsByFilterAdminRequest productFilter,
            CancellationToken cancellation = default)
        {
            var search = productFilter.Search?.ToLower();
            var statusFilter = productFilter.Status;
            var categoryId = productFilter.CategoryId;
            var sortBy = productFilter.SortBy;

            Expression<Func<Product, bool>> filter;

            if (statusFilter.HasValue && statusFilter.Value != ProductAdminStatusFilter.All)
            {
                if (statusFilter.Value == ProductAdminStatusFilter.Approved)
                {
                    filter = x =>
                        (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search)) &&
                        (!categoryId.HasValue || x.CategoryId == categoryId || x.Category.ParentCategoryId == categoryId) &&
                        (x.Status == ProductStatus.Active || x.Status == ProductStatus.Sold || x.Status == ProductStatus.Expired) &&
                        !x.IsDeleted;
                }
                else
                {
                    var domainStatus = (ProductStatus)(int)statusFilter.Value;
                    filter = x =>
                        (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search)) &&
                        (!categoryId.HasValue || x.CategoryId == categoryId || x.Category.ParentCategoryId == categoryId) &&
                        x.Status == domainStatus &&
                        !x.IsDeleted;
                }
            }
            else
            {
                filter = x =>
                    (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search)) &&
                    (!categoryId.HasValue || x.CategoryId == categoryId || x.Category.ParentCategoryId == categoryId) &&
                    !x.IsDeleted;
            }

            Expression<Func<IQueryable<Product>, IOrderedQueryable<Product>>> orderBy = sortBy switch
            {
                ProductAdminSortBy.CreatedAtDesc => q => q.OrderBy(p => p.CreatedAt),
                ProductAdminSortBy.PriceAsc => q => q.OrderBy(p => p.Price),
                ProductAdminSortBy.PriceDesc => q => q.OrderByDescending(p => p.Price),
                _ => q => q.OrderByDescending(p => p.CreatedAt)
            };

            Expression<Func<IQueryable<Product>, IQueryable<Product>>> include = q => q
                .Include(p => p.ProductImages)
                .Include(p => p.Category)
                .Include(p => p.ProductClassification)
                .Include(p => p.Seller)
                .ThenInclude(s => s.Profile);

            (IEnumerable<Product> products, int totalCount) = await _unitOfWork.ProductRepository.GetPagedAsync(
                filter: filter,
                orderBy: orderBy,
                include: include,
                pageNumber: productFilter.PageNumber,
                pageSize: productFilter.PageSize,
                cancellationToken: cancellation);

            List<ProductAdminCardDto> productDtos = products.Select(p => new ProductAdminCardDto
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
            }).ToList();

            return new PaginatedResult<ProductAdminCardDto>(
                productDtos,
                totalCount,
                productFilter.PageNumber,
                productFilter.PageSize);
        }
    }
}
