using MarketplaceSystem.Application.Common.DTOs.Categories;
using MarketplaceSystem.Application.Common.Interfaces.IExternalServices.IStorageServices;
using MarketplaceSystem.Common.Models.Pagination;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;
using System.Linq.Expressions;

namespace MarketplaceSystem.Application.Features.Categories.Queries.GetCategories
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, PaginatedResult<CategoryDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        public GetCategoriesQueryHandler(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }
        public async Task<PaginatedResult<CategoryDto>> Handle(GetCategoriesQuery query, CancellationToken cancellationToken)
        {
            return await GetCategoriesAsync(query.Request, cancellationToken);
        }
        private async Task<PaginatedResult<CategoryDto>> GetCategoriesAsync(GetCategoriesRequest categoryFilter, CancellationToken cancellation = default)
        {
            string search = categoryFilter.Search?.ToLower() ?? string.Empty;
            bool isActive = categoryFilter.IsActive ?? true;

            Expression<Func<Category, bool>>? filter = x =>
                (string.IsNullOrEmpty(search) || x.Name.ToLower().Contains(search)) &&
                (!categoryFilter.IsActive.HasValue || x.IsActive == isActive) &&
                (x.IsDeleted == false);

            (IEnumerable<CategoryDto> categories, int totalCount) = await _unitOfWork.CategoryRepository.GetPagedAsync(
                filter: filter,
                selector: c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Icon = c.Icon != null ? _fileService.GetAbsoluteUrl(c.Icon) : null,
                    DisplayOrder = c.DisplayOrder,
                    IsActive = c.IsActive,
                    ParentCategoryId = c.ParentCategoryId,
                    ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.Name : null,
                    ProductsCount = c.Products != null ? c.Products.Count(p => !p.IsDeleted) : null,
                    Attributes = c.CategoryAttributes!
                        .Where(ca => !ca.IsDeleted)
                        .OrderBy(ca => ca.DisplayOrder)
                        .Select(ca => new CategoryAttributeDto
                        {
                            Id = ca.Id,
                            CategoryId = ca.CategoryId,
                            Name = ca.Name,
                            DisplayName = ca.DisplayName,
                            AttributeType = ca.AttributeType,
                            IsRequired = ca.IsRequired,
                            DisplayOrder = ca.DisplayOrder,
                            Placeholder = ca.Placeholder,
                            Options = ca.AttributeOptions!
                                .Where(ao => !ao.IsDeleted)
                                .OrderBy(ao => ao.Id)
                                .Select(ao => new AttributeOptionDto
                                {
                                    Id = ao.Id,
                                    CategoryAttributeId = ao.CategoryAttributeId,
                                    Value = ao.Value,
                                    IsActive = ao.IsActive,
                                    DisplayText = ao.DisplayText
                                }).ToList()
                        }).ToList(),
                    SubCategories = c.SubCategories!
                        .Where(sc => !sc.IsDeleted)
                        .OrderBy(sc => sc.DisplayOrder)
                        .Select(sc => new CategoryDto
                        {
                            Id = sc.Id,
                            Name = sc.Name,
                            Description = sc.Description,
                            Icon = sc.Icon != null ? _fileService.GetAbsoluteUrl(sc.Icon) : null,
                            DisplayOrder = sc.DisplayOrder,
                            IsActive = sc.IsActive,
                            ParentCategoryId = sc.ParentCategoryId,
                            ParentCategoryName = sc.ParentCategory != null ? sc.ParentCategory.Name : null,
                            ProductsCount = c.Products != null ? c.Products.Count(p => !p.IsDeleted) : null,
                            Attributes = c.CategoryAttributes!
                        .Where(ca => !ca.IsDeleted)
                        .OrderBy(ca => ca.DisplayOrder)
                        .Select(ca => new CategoryAttributeDto
                        {
                            Id = ca.Id,
                            CategoryId = ca.CategoryId,
                            Name = ca.Name,
                            DisplayName = ca.DisplayName,
                            AttributeType = ca.AttributeType,
                            IsRequired = ca.IsRequired,
                            DisplayOrder = ca.DisplayOrder,
                            Placeholder = ca.Placeholder,
                            Options = ca.AttributeOptions!
                                .Where(ao => !ao.IsDeleted)
                                .OrderBy(ao => ao.Id)
                                .Select(ao => new AttributeOptionDto
                                {
                                    Id = ao.Id,
                                    CategoryAttributeId = ao.CategoryAttributeId,
                                    Value = ao.Value,
                                    IsActive = ao.IsActive,
                                    DisplayText = ao.DisplayText
                                }).ToList()
                        }).ToList(),
                            CreatedAt = sc.CreatedAt,
                            UpdatedAt = sc.UpdateAt,
                        }).ToList(),
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdateAt,
                },
                pageNumber: categoryFilter.PageNumber,
                pageSize: categoryFilter.PageSize,
                orderBy: q => q.OrderBy(c => c.DisplayOrder),
                cancellationToken: cancellation
                );

            return new PaginatedResult<CategoryDto>(categories.ToList(), totalCount, categoryFilter.PageNumber, categoryFilter.PageSize);
        }
    }
}
