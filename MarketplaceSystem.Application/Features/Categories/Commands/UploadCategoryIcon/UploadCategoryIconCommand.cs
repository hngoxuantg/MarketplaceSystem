using MediatR;
using Microsoft.AspNetCore.Http;

namespace MarketplaceSystem.Application.Features.Categories.Commands.UploadCategoryIcon
{
    public record UploadCategoryIconCommand(int Id, IFormFile Icon) : IRequest<string>;
}