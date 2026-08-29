using AutoMapper;
using MarketplaceSystem.Application.Common.DTOs.Categories;
using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;
using System.Linq.Expressions;

namespace MarketplaceSystem.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateCategoryCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CategoryDto> Handle(
            CreateCategoryCommand command,
            CancellationToken cancellationToken)
        {
            return await CreateAsync(command.Request, cancellationToken);
        }

        private async Task<CategoryDto> CreateAsync(
            CreateCategoryRequest request,
            CancellationToken cancellation = default)
        {
            if (await _unitOfWork.CategoryRepository.IsExistsAsync(nameof(Category.Name), request.Name))
                throw new ValidatorException(nameof(Category.Name), "Tên danh mục đã tồn tại!");

            if (request.ParentCategoryId.HasValue)
            {
                Category? parentCategory = await _unitOfWork.CategoryRepository.GetByIdAsync(
                    request.ParentCategoryId.Value,
                    cancellation) ?? throw new NotFoundException("Danh mục cha không tồn tại!");

                if (parentCategory.ParentCategoryId.HasValue)
                    throw new ValidatorException(nameof(request.ParentCategoryId), "Chỉ được phép tạo danh mục con cấp 1!");

                CategoryDto result = _mapper.Map<CategoryDto>(await CreateSubCategoryAsync(request, cancellation));

                result.ParentCategoryName = parentCategory.Name;

                return result;
            }
            else
            {
                Category result = await CreateParentCategoryAsync(request, cancellation);
                return _mapper.Map<CategoryDto>(result);
            }
        }
        private async Task<Category> CreateParentCategoryAsync(
            CreateCategoryRequest createCategoryRequest,
            CancellationToken cancellation = default)
        {
            Category? category = _mapper.Map<Category>(createCategoryRequest);

            await GenerateDisplayOrderForCategoryEntityAsync(createCategoryRequest, category, cancellation);

            await _unitOfWork.CategoryRepository.CreateAsync(category, cancellation);

            return category;
        }
        private async Task GenerateDisplayOrderForCategoryEntityAsync(
            CreateCategoryRequest createCategoryRequest,
            Category category,
            CancellationToken cancellation = default)
        {
            if (createCategoryRequest.Attributes == null)
            {
                int orderMax = await _unitOfWork.CategoryRepository.GetOneUntrackedAsync(
                    filter: c => c.ParentCategoryId == null && c.IsDeleted == false,
                    orderBy: q => q.OrderByDescending(c => c.DisplayOrder),
                    selector: c => c.DisplayOrder,
                    cancellation: cancellation);

                if (!createCategoryRequest.DisplayOrder.HasValue || createCategoryRequest.DisplayOrder > orderMax)
                {
                    category.DisplayOrder = orderMax + 1;
                }
                else
                {
                    Expression<Func<Category, bool>> filter =
                        c => c.ParentCategoryId == null &&
                            !c.IsDeleted &&
                            c.DisplayOrder >= createCategoryRequest.DisplayOrder;

                    await ReorderCategoriesAsync(filter, cancellation);

                    category.DisplayOrder = createCategoryRequest.DisplayOrder.Value;
                }
            }
        }
        private async Task GenerateDisplayOrderForSubCategoryAsync(
            CreateCategoryRequest createCategoryRequest,
            Category category,
            CancellationToken cancellation = default)
        {
            int orderMax = await _unitOfWork.CategoryRepository.GetOneUntrackedAsync(
                filter: ca => ca.ParentCategoryId == category.ParentCategoryId && !ca.IsDeleted,
                orderBy: q => q.OrderByDescending(ca => ca.DisplayOrder),
                selector: ca => ca.DisplayOrder,
                cancellation: cancellation);

            if (!createCategoryRequest.DisplayOrder.HasValue || createCategoryRequest.DisplayOrder > orderMax)
            {
                category.DisplayOrder = orderMax + 1;
            }
            else
            {
                await ReorderCategoriesAsync(
                    c => c.ParentCategoryId == createCategoryRequest.ParentCategoryId &&
                        !c.IsDeleted &&
                        c.DisplayOrder >= createCategoryRequest.DisplayOrder,
                    cancellation);

                category.DisplayOrder = createCategoryRequest.DisplayOrder.Value;
            }
        }
        private async Task<Category> CreateSubCategoryAsync(
            CreateCategoryRequest createCategoryRequest,
            CancellationToken cancellation = default)
        {
            Category? category = _mapper.Map<Category>(createCategoryRequest);

            Expression<Func<Category, bool>> filter =
                c => c.ParentCategoryId == createCategoryRequest.ParentCategoryId &&
                    !c.IsDeleted &&
                    c.DisplayOrder >= createCategoryRequest.DisplayOrder;

            await GenerateDisplayOrderForSubCategoryAsync(createCategoryRequest, category, cancellation);

            await _unitOfWork.CategoryRepository.CreateAsync(category, cancellation);

            List<CreateCategoryAttributeRequest>? tempAttr = createCategoryRequest
                .Attributes?
                .OrderBy(a => a.DisplayOrder)
                .ToList();

            if (tempAttr != null && tempAttr.Count > 0)
            {
                for (int i = 0; i < tempAttr?.Count; i++)
                {
                    tempAttr[i].DisplayOrder = i + 1;
                }

                foreach (var attrDto in tempAttr)
                {
                    CategoryAttribute attr = _mapper.Map<CategoryAttribute>(attrDto);

                    attr.AddRangeAttributeOption(_mapper.Map<IEnumerable<AttributeOption>>(attrDto.AttributeOptions));

                    attr.DisplayOrder = attrDto.DisplayOrder;

                    category.AddCategoryAttribute(attr);
                }
            }

            return await _unitOfWork.CategoryRepository.UpdateAsync(category);
        }

        private async Task ReorderCategoriesAsync(
            Expression<Func<Category, bool>> filter,
            CancellationToken cancellation = default)
        {
            List<Category> categories = (await _unitOfWork.CategoryRepository.GetAllAsync<Category>(
                filter: filter,
                orderBy: q => q.OrderBy(c => c.DisplayOrder),
                cancellation: cancellation)).ToList();

            for (int i = 0; i < categories.Count; i++)
            {
                categories[i].DisplayOrder += 1;
            }

            await _unitOfWork.CategoryRepository.UpdateRangeAsync(categories, cancellation);
        }
    }
}
