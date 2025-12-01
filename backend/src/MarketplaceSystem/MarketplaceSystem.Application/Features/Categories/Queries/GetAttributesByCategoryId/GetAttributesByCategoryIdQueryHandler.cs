using MarketplaceSystem.Application.Common.DTOs.Categories;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;
using System.Linq.Expressions;

namespace MarketplaceSystem.Application.Features.Categories.Queries.GetAttributesByCategoryId
{
    public class GetAttributesByCategoryIdQueryHandler : IRequestHandler<GetAttributesByCategoryIdQuery, List<CategoryAttributeDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetAttributesByCategoryIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<CategoryAttributeDto>> Handle(GetAttributesByCategoryIdQuery command, CancellationToken cancellationToken)
        {
            return await GetAttributesByCategoryIdAsync(command.CategoryId, cancellationToken);
        }
        private async Task<List<CategoryAttributeDto>> GetAttributesByCategoryIdAsync(
            int categoryId,
            CancellationToken cancellation = default)
        {
            Expression<Func<CategoryAttribute, CategoryAttributeDto>> selector = ca => new CategoryAttributeDto
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
                    .Select(ao => new AttributeOptionDto
                    {
                        Id = ao.Id,
                        CategoryAttributeId = ao.CategoryAttributeId,
                        Value = ao.Value,
                        IsActive = ao.IsActive,
                        DisplayText = ao.DisplayText
                    }).ToList()
            };

            IEnumerable<CategoryAttributeDto> attributes = await _unitOfWork.CategoryAttributeRepository.GetAllAsync(
                filter: ca => ca.CategoryId == categoryId && !ca.IsDeleted,
                orderBy: q => q.OrderBy(ca => ca.DisplayOrder),
                selector: selector,
                cancellation: cancellation);

            return attributes.ToList();
        }
    }
}
