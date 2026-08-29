using MarketplaceSystem.Application.Common.DTOs.Products;
using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Application.Common.Interfaces.IExternalServices.IStorageServices;
using MarketplaceSystem.Application.Common.Interfaces.IServices;
using MarketplaceSystem.Common.Extensions;
using MarketplaceSystem.Common.Models.Pagination;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;
using System.Linq.Expressions;

namespace MarketplaceSystem.Application.Features.Users.Queries.GetFavorites
{
    public class GetFavoritesQueryHandler : IRequestHandler<GetFavoritesQuery, PaginatedResult<ProductCardDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly IFileService _fileService;
        public GetFavoritesQueryHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUser,
            IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _fileService = fileService;
        }
        public async Task<PaginatedResult<ProductCardDto>> Handle(
            GetFavoritesQuery query,
            CancellationToken cancellationToken)
        {
            return await GetFavoritesAsync(query.UserId, query.Request, cancellationToken);
        }
        private async Task<PaginatedResult<ProductCardDto>> GetFavoritesAsync(
            int userId,
            GetFavoritesRequest filter,
            CancellationToken cancellation = default)
        {
            ValidateUserExists(userId);

            var search = filter.Search?.ToLower();

            Expression<Func<FavoriteProduct, bool>> userFilter = x =>
                (string.IsNullOrEmpty(search) || x.Product.Title.ToLower().Contains(search)) &&
                (x.UserId == userId) &&
                (!x.Product.IsDeleted);

            (IEnumerable<ProductCardDto> products, int totalCount) = await _unitOfWork.FavoriteProductRepository.GetPagedAsync<ProductCardDto>(
                filter: userFilter,
                selector: p => new ProductCardDto
                {
                    Id = p.Product.Id,
                    Title = p.Product.Title,
                    Price = p.Product.Price,
                    ImageUrl = p.Product.ProductImages.Count > 0 ? _fileService
                    .GetAbsoluteUrl(
                        (p.Product.ProductImages
                            .Where(p => p.IsMain)
                            .Select(pi => pi.ImageUrl)
                            .FirstOrDefault())) : null,
                    Condition = p.Product.Condition.GetDisplayName(),
                    Location = p.Product.Location.GetDisplayName(),
                    CategoryName = p.Product.Category != null ? p.Product.Category.Name : "",
                    CreateAt = p.Product.CreatedAt.ToVietnamTime()
                },
                orderBy: q => q.OrderByDescending(fp => fp.CreateAt),
                pageNumber: filter.PageNumber,
                pageSize: filter.PageSize,
                cancellationToken: cancellation);

            return new PaginatedResult<ProductCardDto>(products.ToList(), totalCount, filter.PageNumber, filter.PageSize);
        }
        private void ValidateUserExists(int userId)
        {
            if (userId != _currentUser.UserId)
            {
                throw new ForbiddenAccessException("Bạn không được phép truy cập vào tài nguyên này!");
            }
        }
    }
}
