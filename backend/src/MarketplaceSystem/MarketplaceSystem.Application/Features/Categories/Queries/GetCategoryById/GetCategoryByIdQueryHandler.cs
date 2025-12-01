using MarketplaceSystem.Application.Common.DTOs.Categories;
using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Application.Common.Interfaces.IExternalServices.IStorageServices;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Categories.Queries.GetCategoryById
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;
        public GetCategoryByIdQueryHandler(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }
        public async Task<CategoryDto> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken)
        {
            return await GetCategoryByIdAsync(query.Id, cancellationToken);
        }
        private async Task<CategoryDto> GetCategoryByIdAsync(int categoryId, CancellationToken cancellation = default)
        {
            return await _unitOfWork.CategoryRepository.GetOneUntrackedAsync(
                filter: c => c.Id == categoryId && !c.IsDeleted,
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
                cancellation: cancellation) ?? throw new NotFoundException("Không tìm thấy danh mucS.");
        }
    }
}
