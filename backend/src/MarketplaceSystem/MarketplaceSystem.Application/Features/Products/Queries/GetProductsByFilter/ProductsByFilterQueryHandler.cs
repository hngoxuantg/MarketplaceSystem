using MarketplaceSystem.Application.Common.DTOs.Products;
using MarketplaceSystem.Application.Common.Interfaces.IExternalServices.IStorageServices;
using MarketplaceSystem.Application.Common.Interfaces.IServices;
using MarketplaceSystem.Common.Enums;
using MarketplaceSystem.Common.Extensions;
using MarketplaceSystem.Common.Models.Pagination;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Enums.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;
using System.Linq.Expressions;

namespace MarketplaceSystem.Application.Features.Products.Queries.GetProductsByFilter
{
    public class ProductsByFilterQueryHandler : IRequestHandler<ProductsByFilterQuery, PaginatedResult<ProductCardDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileService _fileService;
        public ProductsByFilterQueryHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _fileService = fileService;
        }
        public async Task<PaginatedResult<ProductCardDto>> Handle(
            ProductsByFilterQuery query,
            CancellationToken cancellationToken)
        {
            return await GetProductsByFilter(query.Request, cancellationToken);
        }
        private async Task<PaginatedResult<ProductCardDto>> GetProductsByFilter(
            ProductsByFilterRequest request,
            CancellationToken cancellation = default)
        {
            VietnamProvince? userLocation = null;
            if (_currentUserService.IsAuthenticated)
            {
                userLocation = await _unitOfWork.UserProfileRepository.GetOneUntrackedAsync(
                    filter: u => u.UserId == _currentUserService.UserId,
                    selector: u => u.Location,
                    cancellation: cancellation);
            }

            var search = request.Search?.ToLower();
            var location = request.Location;
            var categoryId = request.CategoryId;

            Expression<Func<Product, bool>> filter = x =>
                (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search)) &&
                (!location.HasValue || x.Location == location) &&
                (!categoryId.HasValue || x.CategoryId == categoryId || x.Category.ParentCategoryId == categoryId) &&
                (!x.IsDeleted) && (x.Status == ProductStatus.Active);

            Expression<Func<IQueryable<Product>, IOrderedQueryable<Product>>> orderBy = p
                => p.OrderByDescending(p => p.Location == userLocation)
                    .ThenByDescending(p => p.ViewCount)
                    .ThenByDescending(p => p.CreatedAt);

            var (products, totalCount) = await _unitOfWork.ProductRepository.GetPagedAsync(
            filter: filter,
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            orderBy: _currentUserService.IsAuthenticated ? orderBy : null,
            selector: p => new ProductCardDto
            {
                Id = p.Id,
                Title = p.Title,
                Price = p.Price,
                ImageUrl = p.ProductImages.Count > 0 ? _fileService
                    .GetAbsoluteUrl(
                        (p.ProductImages
                            .Where(p => p.IsMain)
                            .Select(pi => pi.ImageUrl)
                            .FirstOrDefault())) : null,
                Condition = p.Condition.GetDisplayName(),
                Location = p.Location.GetDisplayName(),
                CategoryName = p.Category != null ? p.Category.Name : "",
                CreateAt = p.CreatedAt.ToVietnamTime()
            });

            return new PaginatedResult<ProductCardDto>(
                products,
                totalCount,
                request.PageNumber,
                request.PageSize);
        }
    }
}
