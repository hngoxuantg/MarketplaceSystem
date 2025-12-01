using MarketplaceSystem.Application.Common.DTOs.Categories;
using MediatR;

namespace MarketplaceSystem.Application.Features.Categories.Commands.UpdateCategory
{
    public record UpdateCategoryCommand(int Id, UpdateCategoryRequest Request) : IRequest<CategoryDto>;
}
