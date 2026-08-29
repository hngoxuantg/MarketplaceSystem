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

namespace MarketplaceSystem.Application.Features.Users.Queries.GetProducts
{
    public class UserProductsFilterQueryHandler : IRequestHandler<UserProductsFilterQuery, PaginatedResult<ProductCardDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUser;
        private readonly IFileService _fileService;
        public UserProductsFilterQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _fileService = fileService;
        }
        public async Task<PaginatedResult<ProductCardDto>> Handle(UserProductsFilterQuery query, CancellationToken cancellationToken)
        {
            return await GetProducts(query.UserId, query.Request, cancellationToken);
        }
        private async Task<PaginatedResult<ProductCardDto>> GetProducts(int userId, UserProductsFilterRequest filter, CancellationToken cancellation = default)
        {
            ValidateUserExists(userId);

            var search = filter.Search?.ToLower();

            Expression<Func<Product, bool>> userFilter = x =>
                (string.IsNullOrEmpty(search) || x.Title.ToLower().Contains(search)) &&
                (x.SellerId == userId) &&
                (!x.IsDeleted);

            (IEnumerable<ProductCardDto> products, int totalCount) = await _unitOfWork.ProductRepository.GetPagedAsync(
                filter: userFilter,
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
                    Status = p.Status.GetDisplayName(),
                    Location = p.Location.GetDisplayName(),
                    CategoryName = p.Category != null ? p.Category.Name : "",
                    CreateAt = p.CreatedAt.ToVietnamTime()
                },
                orderBy: q => q.OrderByDescending(p => p.CreatedAt),
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
