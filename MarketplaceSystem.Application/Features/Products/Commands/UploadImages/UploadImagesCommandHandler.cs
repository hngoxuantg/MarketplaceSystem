using AutoMapper;
using MarketplaceSystem.Application.Common.DTOs.Products;
using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Application.Common.Interfaces.IExternalServices.IAIServices;
using MarketplaceSystem.Application.Common.Interfaces.IExternalServices.IStorageServices;
using MarketplaceSystem.Application.Common.Interfaces.IServices;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Enums.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceSystem.Application.Features.Products.Commands.UploadImages
{
    public class UploadImagesCommandHandler : IRequestHandler<UploadImagesCommand, ProductImagesDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        private readonly IGeminiAIService _geminiAIService;
        public UploadImagesCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IFileService fileService,
            IMapper mapper,
            IGeminiAIService geminiAIService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _fileService = fileService;
            _mapper = mapper;
            _geminiAIService = geminiAIService;
        }
        public async Task<ProductImagesDto> Handle(UploadImagesCommand command, CancellationToken cancellationToken)
        {
            return await UploadImagesAysnc(command.Request, cancellationToken);
        }

        private async Task<ProductImagesDto> UploadImagesAysnc(UploadImagesRequest request, CancellationToken cancellation = default)
        {
            await _unitOfWork.BeginTransactionAsync(cancellation);
            try
            {
                Product? product = await GetProductByIdAsync(request.ProductId, cancellation);

                IFormFile mainFile = request.Images[request.IsMainIndex];

                (ContentWarningFlag? flag, string? detail) = await _geminiAIService.CheckProductContentAsync(
                    product.Title,
                    product.Description ?? string.Empty,
                    request.Images,
                    cancellation);

                string mainUrl = await _fileService.SaveImageAsync(
                    mainFile,
                    "products",
                    cancellation);

                product.SetProductClassification(flag ?? ContentWarningFlag.None, detail);

                request.Images.RemoveAt(request.IsMainIndex);

                List<string> urls = await _fileService.SaveImagesAsync(request.Images, "products", cancellation);
                product.AddImage(new ProductImage
                {
                    ImageUrl = mainUrl,
                    IsMain = true
                });

                foreach (var url in urls)
                {
                    product.AddImage(new ProductImage
                    {
                        ImageUrl = url,
                        IsMain = false
                    });
                }

                await _unitOfWork.ProductRepository.UpdateAsync(product, cancellation);

                List<ProductImageDto> imageDtos = _mapper.Map<List<ProductImageDto>>(product.ProductImages);
                imageDtos.ForEach(img =>
                {
                    img.ImageUrl = _fileService.GetAbsoluteUrl(img.ImageUrl);
                });

                await _unitOfWork.CommitTransactionAsync(cancellation);

                return new ProductImagesDto
                {
                    TotalImages = product.ProductImages.Count,
                    Images = imageDtos
                };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellation);
                throw;
            }
        }
        private async Task<Product> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.ProductRepository.GetOneAsync<Product>(
                filter: p => p.Id == id && !p.IsDeleted && p.SellerId == _currentUserService.UserId,
                include: p => p.Include(p => p.ProductImages),
                cancellation: cancellationToken
                ) ?? throw new NotFoundException("Sản phẩm không tồn tại");
        }
    }
}
