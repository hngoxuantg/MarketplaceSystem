using MarketplaceSystem.Application.Common.DTOs.Categories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Categories.Commands.CreateCategory
{
    public record CreateCategoryCommand(CreateCategoryRequest Request) : IRequest<CategoryDto>;
}
