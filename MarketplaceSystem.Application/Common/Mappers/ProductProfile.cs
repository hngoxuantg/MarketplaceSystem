using AutoMapper;
using MarketplaceSystem.Application.Common.DTOs.Products;
using MarketplaceSystem.Application.Features.Products.Commands.CreateProduct;
using MarketplaceSystem.Common.Extensions;
using MarketplaceSystem.Domain.Entities.Business;

namespace MarketplaceSystem.Application.Common.Mappers
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<CreateProductRequest, Product>()
                .ConstructUsing(src => new Product(
                    src.Title,
                    src.Description,
                    src.Price,
                    src.Condition,
                    src.Quantity,
                    src.Location,
                    src.CategoryId))
                .ForMember(p => p.ProductAttributeValues,
                opt => opt.Ignore());

            CreateMap<CreateProductAttributeValueRequest, ProductAttributeValue>()
                .ConstructUsing(src => new ProductAttributeValue(
                    src.CategoryAttributeId,
                    src.TextValue,
                    src.NumberValue,
                    src.BooleanValue,
                    src.DateValue,
                    src.SelectValues));

            CreateMap<ProductImage, ProductImageDto>()
                .ForMember(p => p.Id,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(p => p.ImageUrl,
                opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(p => p.IsMain,
                opt => opt.MapFrom(src => src.IsMain));

            CreateMap<Product, ProductCardDto>()
                .ForMember(p => p.Id,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(p => p.Title,
                opt => opt.MapFrom(src => src.Title))
                .ForMember(p => p.Price,
                opt => opt.MapFrom(src => src.Price))
                .ForMember(p => p.Condition,
                opt => opt.MapFrom(src => src.Condition.GetDisplayName()))
                .ForMember(p => p.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(p => p.Location,
                opt => opt.MapFrom(src => src.Location.GetDisplayName()))
                .ForMember(p => p.CreateAt,
                opt => opt.MapFrom(src => src.CreatedAt.ToVietnamTime()));

            CreateMap<Product, ProductDto>()
                .ForMember(p => p.Id,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(p => p.Title,
                opt => opt.MapFrom(src => src.Title))
                .ForMember(p => p.Description,
                opt => opt.MapFrom(src => src.Description))
                .ForMember(p => p.Price,
                opt => opt.MapFrom(src => src.Price))
                .ForMember(p => p.Quantity,
                opt => opt.MapFrom(src => src.Quantity))
                .ForMember(p => p.Condition,
                opt => opt.MapFrom(src => src.Condition.GetDisplayName()))
                .ForMember(p => p.ProductStatus,
                opt => opt.MapFrom(src => src.Status.GetDisplayName()))
                .ForMember(p => p.CategoryId,
                opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(p => p.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(p => p.Location,
                opt => opt.MapFrom(src => src.Location.GetDisplayName()))
                .ForMember(p => p.Images,
                opt => opt.Ignore())
                .ForMember(p => p.CreatedAt,
                opt => opt.MapFrom(src => src.CreatedAt.ToVietnamTime()))
                .ForMember(p => p.Seller,
                opt => opt.MapFrom(src => src.Seller))
                .ForMember(p => p.AttributeValues,
                opt => opt.MapFrom(src => src.ProductAttributeValues));

            CreateMap<ProductAttributeValue, ProductAttributeValueDto>()
                .ForMember(p => p.Id,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(p => p.AttributeType,
                opt => opt.MapFrom(src => src.CategoryAttribute.AttributeType))
                .ForMember(p => p.AttributeName,
                opt => opt.MapFrom(src => src.CategoryAttribute.DisplayName))
                .ForMember(p => p.TextValue,
                opt => opt.MapFrom(src => src.TextValue))
                .ForMember(p => p.NumberValue,
                opt => opt.MapFrom(src => src.NumberValue))
                .ForMember(p => p.BooleanValue,
                opt => opt.MapFrom(src => src.BooleanValue))
                .ForMember(p => p.DateValue,
                opt => opt.MapFrom(src => src.DateValue))
                .ForMember(p => p.SelectsValue,
                opt => opt.MapFrom(src => src.CategoryAttribute.AttributeOptions
                    .FirstOrDefault(ao => ao.CategoryAttributeId == src.CategoryAttributeId).DisplayText));
        }
    }
}
