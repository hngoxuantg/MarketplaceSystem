using MarketplaceSystem.Application.Common.DTOs.Products;
using MediatR;

namespace MarketplaceSystem.Application.Features.Products.Commands.UploadImages
{
    public record UploadImagesCommand(UploadImagesRequest Request) : IRequest<ProductImagesDto>;
}
