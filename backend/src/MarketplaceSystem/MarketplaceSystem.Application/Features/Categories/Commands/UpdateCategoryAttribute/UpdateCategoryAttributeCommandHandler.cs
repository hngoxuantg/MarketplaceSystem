using AutoMapper;
using MarketplaceSystem.Application.Common.DTOs.Categories;
using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceSystem.Application.Features.Categories.Commands.UpdateCategoryAttribute
{
    public class UpdateCategoryAttributeCommandHandler : IRequestHandler<UpdateCategoryAttributeCommand, CategoryAttributeDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UpdateCategoryAttributeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<CategoryAttributeDto> Handle(UpdateCategoryAttributeCommand command, CancellationToken cancellationToken)
        {
            return await UpdateCategoryAttributeAsync(command.CategoryId, command.AttributeId, command.Request, cancellationToken);
        }


        private async Task<CategoryAttributeDto> UpdateCategoryAttributeAsync(
            int categoryId,
            int attributeId,
            UpdateCategoryAttributeRequest request,
            CancellationToken cancellation = default)
        {
            await ValidateCategoryAndAttributeAsync(categoryId, request, cancellation);

            CategoryAttribute categoryAttribute = await GetCategoryAttributeAsync(attributeId, cancellation);

            if (categoryAttribute.DisplayOrder != request.DisplayOrder)
            {
                await ReorderCategoryAttributesAsync(categoryAttribute, request.DisplayOrder, cancellation);
            }

            _mapper.Map(request, categoryAttribute);

            await _unitOfWork.CategoryAttributeRepository.UpdateAsync(categoryAttribute, cancellation);

            return _mapper.Map<CategoryAttributeDto>(categoryAttribute);
        }
        private async Task<CategoryAttribute> GetCategoryAttributeAsync(int attributeId, CancellationToken cancellation)
        {
            CategoryAttribute categoryAttribute = await _unitOfWork.CategoryAttributeRepository.GetOneAsync<CategoryAttribute>(
                filter: ca => ca.Id == attributeId && !ca.IsDeleted,
                include: ca => ca.Include(ca => ca.AttributeOptions),
                cancellation: cancellation
            ) ?? throw new NotFoundException("Thuộc tính danh mục không tồn tại!");

            return categoryAttribute;
        }
        private async Task ValidateCategoryAndAttributeAsync(int categoryId, UpdateCategoryAttributeRequest request, CancellationToken cancellation)
        {
            if (!await _unitOfWork.CategoryRepository.IsExistsAsync(nameof(Category.Id), categoryId, cancellation))
                throw new NotFoundException("Danh mục không tồn tại!");
        }
        private async Task ReorderCategoryAttributesAsync(CategoryAttribute categoryAttribute, int newDisplayOrder, CancellationToken cancellation)
        {
            int maxDisplayOrder = await _unitOfWork.CategoryAttributeRepository.GetOneUntrackedAsync(
                filter: ca => ca.CategoryId == categoryAttribute.CategoryId && !ca.IsDeleted,
                orderBy: q => q.OrderByDescending(ca => ca.DisplayOrder),
                selector: ca => ca.DisplayOrder,
                cancellation: cancellation);

            if (newDisplayOrder > maxDisplayOrder)
            {
                List<CategoryAttribute> attributesToUpdate = (await _unitOfWork.CategoryAttributeRepository.GetAllAsync<CategoryAttribute>(
                    filter: ca => ca.CategoryId == categoryAttribute.CategoryId && ca.DisplayOrder > categoryAttribute.DisplayOrder && !ca.IsDeleted,
                    cancellation: cancellation)).ToList();

                foreach (CategoryAttribute ca in attributesToUpdate)
                {
                    ca.DisplayOrder -= 1;
                }

                await _unitOfWork.CategoryAttributeRepository.UpdateRangeAsync(attributesToUpdate, cancellation);

                categoryAttribute.DisplayOrder = maxDisplayOrder;
            }
            else
            {
                CategoryAttribute? oldAttribute = await _unitOfWork.CategoryAttributeRepository.GetOneAsync<CategoryAttribute>(
                    filter: ca => ca.CategoryId == categoryAttribute.CategoryId && ca.DisplayOrder == newDisplayOrder && !ca.IsDeleted,
                    cancellation: cancellation);

                if (oldAttribute == null)
                {
                    categoryAttribute.DisplayOrder = newDisplayOrder;
                }
                else
                {
                    int temp = oldAttribute.DisplayOrder;
                    oldAttribute.DisplayOrder = categoryAttribute.DisplayOrder;
                    categoryAttribute.DisplayOrder = temp;

                    await _unitOfWork.CategoryAttributeRepository.UpdateAsync(oldAttribute, cancellation);
                }
            }
        }
    }
}
