using MarketplaceSystem.Application.Common.DTOs.Categories;
using MarketplaceSystem.Application.Common.Interfaces.IExternalServices.IStorageServices;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Categories.Queries.GetListingCategories
{
    public class GetListingCategoriesQueryHandler : IRequestHandler<GetListingCategoriesQuery, List<ListingCategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        public GetListingCategoriesQueryHandler(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }
        public async Task<List<ListingCategoryDto>> Handle(GetListingCategoriesQuery query, CancellationToken cancellationToken)
        {
            return await GetListingCategoriesAsync(cancellationToken);
        }
        private async Task<List<ListingCategoryDto>> GetListingCategoriesAsync(CancellationToken cancellation = default)
        {
            IEnumerable<Category> allCategories = await _unitOfWork.CategoryRepository.GetAllAsync<Category>(
                filter: c => c.IsActive && !c.IsDeleted,
                orderBy: q => q.OrderBy(c => c.DisplayOrder),
                cancellation: cancellation);

            ILookup<int?, Category> lookup = allCategories.ToLookup(c => c.ParentCategoryId);

            List<ListingCategoryDto> listingCategories = lookup[null]
                .Select(parent => new ListingCategoryDto
                {
                    Id = parent.Id,
                    Name = parent.Name,
                    Description = parent.Description,
                    IconUrl = parent.Icon != null ? _fileService.GetAbsoluteUrl(parent.Icon) : null,
                    DisplayOrder = parent.DisplayOrder,
                    Subcategories = lookup[parent.Id]
                        .Select(sub => new ListingSubcategoryDto
                        {
                            Id = sub.Id,
                            ParentCategoryId = sub.ParentCategoryId ?? 0,
                            Name = sub.Name,
                            IconUrl = sub.Icon != null ? _fileService.GetAbsoluteUrl(sub.Icon) : null,
                            DisplayOrder = sub.DisplayOrder
                        }).ToList()
                })
                .ToList();

            return listingCategories;
        }
    }
}
