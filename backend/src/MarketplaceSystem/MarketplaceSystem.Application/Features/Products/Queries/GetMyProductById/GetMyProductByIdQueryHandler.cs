using AutoMapper;
using MarketplaceSystem.Application.Common.DTOs.Products;
using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Application.Common.Interfaces.IExternalServices.IStorageServices;
using MarketplaceSystem.Application.Common.Interfaces.IServices;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Enums.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceSystem.Application.Features.Products.Queries.GetMyProductById
{
    public class GetMyProductByIdQueryHandler : IRequestHandler<GetMyProductByIdQuery, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;

        public GetMyProductByIdQueryHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IFileService fileService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _fileService = fileService;
            _mapper = mapper;
        }

        public async Task<ProductDto> Handle(GetMyProductByIdQuery query, CancellationToken cancellationToken)
        {
            Product product = await _unitOfWork.ProductRepository.GetOneUntrackedAsync<Product>(
                filter: p => p.Id == query.ProductId && !p.IsDeleted,
                include: p => p
                    .Include(p => p.Category)
                    .Include(p => p.ProductImages)
                    .Include(p => p.Seller!)
                    .ThenInclude(p => p.Profile)
                    .Include(p => p.ProductAttributeValues)
                    .ThenInclude(p => p.CategoryAttribute!)
                    .ThenInclude(p => p.AttributeOptions),
                cancellation: cancellationToken
            ) ?? throw new NotFoundException("Sản phẩm không tồn tại");

            if (product.SellerId != _currentUserService.UserId)
            {
                throw new ForbiddenAccessException("Bạn không có quyền xem sản phẩm này");
            }

            ProductDto productDto = _mapper.Map<ProductDto>(product);

            productDto.Images = new ProductImagesDto
            {
                TotalImages = product.ProductImages.Count,
                Images = _mapper.Map<List<ProductImageDto>>(product.ProductImages)
            };

            foreach (var img in productDto.Images.Images)
            {
                img.ImageUrl = _fileService.GetAbsoluteUrl(img.ImageUrl);
            }

            int totalProductsBySeller = await _unitOfWork.ProductRepository.GetCountAsync(
                filter: p => p.SellerId == _currentUserService.UserId && !p.IsDeleted,
                cancellation: cancellationToken);

            int totalSoldBySeller = await _unitOfWork.ProductRepository.GetCountAsync(
                filter: p => p.SellerId == _currentUserService.UserId && !p.IsDeleted && p.Status == ProductStatus.Sold,
                cancellation: cancellationToken);

            productDto.Seller.TotalProducts = totalProductsBySeller;
            productDto.Seller.TotalSold = totalSoldBySeller;

            return productDto;
        }
    }
}
