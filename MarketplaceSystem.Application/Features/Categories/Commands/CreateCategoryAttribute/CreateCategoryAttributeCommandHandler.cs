using AutoMapper;
using MarketplaceSystem.Application.Common.DTOs.Categories;
using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MarketplaceSystem.Application.Features.Categories.Commands.CreateCategoryAttribute
{
    public class CreateCategoryAttributeCommandHandler : IRequestHandler<CreateCategoryAttributeCommand, CategoryAttributeDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CreateCategoryAttributeCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<CategoryAttributeDto> Handle(CreateCategoryAttributeCommand command, CancellationToken cancellationToken)
        {
            return await CreateCategoryAttributeAsync(command.CategoryId, command.Request, cancellationToken);
        }
        private async Task<CategoryAttributeDto> CreateCategoryAttributeAsync(
            int id,
            CreateCategoryAttributeRequest createCategoryAttributeRequest,
            CancellationToken cancellation = default)
        {
            Category? category = await _unitOfWork.CategoryRepository.GetOneAsync<Category>(
                filter: c => c.Id == id && !c.IsDeleted,
                include: c => c.Include(c => c.CategoryAttributes),
                cancellation: cancellation)
                ?? throw new NotFoundException("Danh mục không tồn tại!");

            CategoryAttribute? categoryAttribute = _mapper.Map<CategoryAttribute>(createCategoryAttributeRequest);

            if (category.ParentCategoryId == null)
                throw new ValidatorException(nameof(Category.ParentCategoryId), "Chỉ được phép thêm thuộc tính cho danh mục con!");
            if (category.CategoryAttributes.Any(ca => ca.Name == createCategoryAttributeRequest.Name))
                throw new ValidatorException(nameof(CategoryAttribute.Name), "Tên thuộc tính đã tồn tại trong danh mục này!");

            int displayOrderMax = category.CategoryAttributes.Any()
                ? category.CategoryAttributes.Max(ca => ca.DisplayOrder)
                : 0;

            if (createCategoryAttributeRequest.DisplayOrder >= displayOrderMax)
            {
                categoryAttribute.DisplayOrder = displayOrderMax + 1;
            }
            else
            {
                Expression<Func<CategoryAttribute, bool>> filter =
                    ca => ca.CategoryId == category.Id &&
                        !ca.IsDeleted &&
                        ca.DisplayOrder >= createCategoryAttributeRequest.DisplayOrder;
                List<CategoryAttribute> categoryAttributesToReorder = category.CategoryAttributes
                    .Where(filter.Compile())
                    .OrderBy(ca => ca.DisplayOrder)
                    .ToList();
                for (int i = 0; i < categoryAttributesToReorder.Count; i++)
                {
                    categoryAttributesToReorder[i].DisplayOrder += 1;
                }
                await _unitOfWork.CategoryAttributeRepository.UpdateRangeAsync(categoryAttributesToReorder, cancellation);
                categoryAttribute.DisplayOrder = createCategoryAttributeRequest.DisplayOrder;

                category.AddCategoryAttribute(categoryAttribute);
                await _unitOfWork.CategoryRepository.UpdateAsync(category, cancellation);
            }

            return _mapper.Map<CategoryAttributeDto>(categoryAttribute);
        }
    }
}
