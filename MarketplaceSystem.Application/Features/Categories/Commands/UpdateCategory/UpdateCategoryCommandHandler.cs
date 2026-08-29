using AutoMapper;
using MarketplaceSystem.Application.Common.DTOs.Categories;
using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceSystem.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UpdateCategoryCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CategoryDto> Handle(
            UpdateCategoryCommand command,
            CancellationToken cancellationToken)
        {
            return await UpdateCategoryAsync(
                command.Id,
                command.Request,
                cancellationToken);
        }
        private async Task<CategoryDto> UpdateCategoryAsync(
            int categoryId,
            UpdateCategoryRequest request,
            CancellationToken cancellation = default)
        {
            await ValidateCategoryForUpdateAsync(categoryId, request, cancellation);

            Category category = await GetCategoryAsync(categoryId, cancellation);

            _mapper.Map(request, category);

            if (category.DisplayOrder != request.DisplayOrder)
            {
                await ReorderCategoriesAsync(
                    category,
                    request.DisplayOrder,
                    category.ParentCategoryId,
                    cancellation);
            }

            await _unitOfWork.CategoryRepository.UpdateAsync(category, cancellation);

            return _mapper.Map<CategoryDto>(category);
        }
        private async Task ValidateCategoryForUpdateAsync(int categoryId, UpdateCategoryRequest request, CancellationToken cancellation)
        {
            if (await _unitOfWork.CategoryRepository.IsExistsForUpdateAsync(
                categoryId, nameof(Category.Name), request.Name, cancellation))
            {
                throw new BusinessRuleException("Danh mục với tên đã tồn tại!");
            }
        }
        private async Task<Category> GetCategoryAsync(int categoryId, CancellationToken cancellation)
        {
            Category category = await _unitOfWork.CategoryRepository.GetOneAsync<Category>(
                filter: c => c.Id == categoryId && !c.IsDeleted,
                include: c => c.Include(c => c.CategoryAttributes)
                                .ThenInclude(ca => ca.AttributeOptions),
                cancellation: cancellation
            ) ?? throw new NotFoundException("Không tìm thấy danh mục!");

            return category;
        }
        private async Task ReorderCategoriesAsync(
            Category category,
            int newDisplayOrder,
            int? parentCategoryId,
            CancellationToken cancellation)
        {
            int maxDisplayOrder = await _unitOfWork.CategoryRepository.GetOneUntrackedAsync(
                filter: ca => ca.ParentCategoryId == parentCategoryId && !ca.IsDeleted,
                orderBy: q => q.OrderByDescending(ca => ca.DisplayOrder),
                selector: ca => ca.DisplayOrder,
                cancellation: cancellation);

            if (newDisplayOrder > maxDisplayOrder)
            {
                List<Category> categoriesToUpdate = (await _unitOfWork.CategoryRepository.GetAllAsync<Category>(
                    filter: c => c.ParentCategoryId == parentCategoryId && c.DisplayOrder > category.DisplayOrder && !c.IsDeleted,
                    cancellation: cancellation)).ToList();

                foreach (Category cat in categoriesToUpdate)
                {
                    cat.DisplayOrder -= 1;
                }

                await _unitOfWork.CategoryRepository.UpdateRangeAsync(categoriesToUpdate, cancellation);

                category.DisplayOrder = maxDisplayOrder;
            }
            else
            {
                Category? oldCategory = await _unitOfWork.CategoryRepository.GetOneAsync<Category>(
                    filter: c => c.DisplayOrder == newDisplayOrder && !c.IsDeleted,
                    cancellation: cancellation);

                if (oldCategory == null)
                {
                    category.DisplayOrder = newDisplayOrder;
                }
                else
                {
                    int temp = oldCategory.DisplayOrder;
                    oldCategory.DisplayOrder = category.DisplayOrder;
                    category.DisplayOrder = temp;

                    await _unitOfWork.CategoryRepository.UpdateAsync(oldCategory, cancellation);
                }
            }
        }
    }
}
