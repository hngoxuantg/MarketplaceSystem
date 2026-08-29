using AutoMapper;
using MarketplaceSystem.Application.Common.DTOs.Categories;
using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceSystem.Application.Features.Categories.Commands.UpdateAttributeOption
{
    public class UpdateAttributeOptionCommandHandler : IRequestHandler<UpdateAttributeOptionCommand, AttributeOptionDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UpdateAttributeOptionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AttributeOptionDto> Handle(UpdateAttributeOptionCommand command, CancellationToken cancellationToken)
        {
            return await UpdateAttributeOptionAsync(
                command.Request,
                command.CategoryId,
                command.AttributeId,
                command.OptionId,
                cancellationToken);
        }
        private async Task<AttributeOptionDto> UpdateAttributeOptionAsync(
            UpdateAttributeOptionRequest request,
            int categoryId,
            int categoryAttributeId,
            int attributeOptionId,
            CancellationToken cancellation = default)
        {
            Category category = await GetCategoryAsync(categoryId, cancellation);

            if (category.CategoryAttributes == null || !category.CategoryAttributes.Any(c => c.Id == categoryAttributeId))
                throw new NotFoundException("Thuộc tính danh mục không tồn tại!");
            if (category.CategoryAttributes.First(c => c.Id == categoryAttributeId).AttributeOptions == null ||
                !category.CategoryAttributes.First(c => c.Id == categoryAttributeId).AttributeOptions.Any(ao => ao.Id == attributeOptionId))
                throw new NotFoundException("Tùy chọn thuộc tính không tồn tại!");

            AttributeOption attributeOption = category.CategoryAttributes.First(c => c.Id == categoryAttributeId)
                .AttributeOptions.First(ao => ao.Id == attributeOptionId);

            _mapper.Map(request, attributeOption);

            await _unitOfWork.AttributeOptionRepository.UpdateAsync(attributeOption, cancellation);
            return _mapper.Map<AttributeOptionDto>(attributeOption);
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
    }
}
