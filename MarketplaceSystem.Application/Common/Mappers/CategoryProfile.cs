using AutoMapper;
using MarketplaceSystem.Application.Common.DTOs.Categories;
using MarketplaceSystem.Application.Features.Categories.Commands.CreateCategory;
using MarketplaceSystem.Application.Features.Categories.Commands.UpdateAttributeOption;
using MarketplaceSystem.Application.Features.Categories.Commands.UpdateCategory;
using MarketplaceSystem.Application.Features.Categories.Commands.UpdateCategoryAttribute;
using MarketplaceSystem.Domain.Entities.Business;

namespace MarketplaceSystem.Application.Common.Mappers
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<CreateCategoryRequest, Category>()
                .ConstructUsing(src => new Category(
                    src.Name, src.Description, src.IsActive, src.ParentCategoryId))
                .ForMember(dest => dest.DisplayOrder,
                opt => opt.Ignore())
                .ForMember(dest => dest.CategoryAttributes,
                opt => opt.Ignore());

            CreateMap<CreateCategoryAttributeRequest, CategoryAttribute>()
                .ConstructUsing(src => new CategoryAttribute(
                    src.Name, src.DisplayName, src.AttributeType, src.IsRequired, src.Placeholder))
                .ForMember(dest => dest.DisplayOrder,
                opt => opt.Ignore())
                .ForMember(dest => dest.AttributeOptions,
                opt => opt.Ignore());

            CreateMap<CreateAttributeOptionRequest, AttributeOption>()
                .ConstructUsing(src => new AttributeOption(
                    src.Value, src.DisplayText));

            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description,
                opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Icon,
                opt => opt.MapFrom(src => src.Icon))
                .ForMember(dest => dest.DisplayOrder,
                opt => opt.MapFrom(src => src.DisplayOrder))
                .ForMember(dest => dest.IsActive,
                opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.ParentCategoryId,
                opt => opt.MapFrom(src => src.ParentCategoryId))
                .ForMember(dest => dest.ParentCategoryName,
                opt => opt.Ignore())
                .ForMember(dest => dest.Attributes,
                opt => opt.MapFrom(src => src.CategoryAttributes));

            CreateMap<CategoryAttribute, CategoryAttributeDto>()
                .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CategoryId,
                opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.DisplayName,
                opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.AttributeType,
                opt => opt.MapFrom(src => src.AttributeType))
                .ForMember(dest => dest.IsRequired,
                opt => opt.MapFrom(src => src.IsRequired))
                .ForMember(dest => dest.DisplayOrder,
                opt => opt.MapFrom(src => src.DisplayOrder))
                .ForMember(dest => dest.Placeholder,
                opt => opt.MapFrom(src => src.Placeholder))
                .ForMember(dest => dest.Options,
                opt => opt.MapFrom(src => src.AttributeOptions));

            CreateMap<AttributeOption, AttributeOptionDto>()
                .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Value,
                opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.DisplayText,
                opt => opt.MapFrom(src => src.DisplayText))
                .ForMember(dest => dest.IsActive,
                opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.CategoryAttributeId,
                opt => opt.MapFrom(src => src.CategoryAttributeId));

            CreateMap<Category, RootCategoryItemListDto>()
                .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Icon,
                opt => opt.MapFrom(src => src.Icon))
                .ForMember(dest => dest.DisplayOrder,
                opt => opt.MapFrom(src => src.DisplayOrder));

            CreateMap<UpdateCategoryRequest, Category>()
                .ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description,
                opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ParentCategoryId,
                opt => opt.MapFrom(src => src.ParentCategoryId))
                .ForMember(dest => dest.DisplayOrder,
                opt => opt.Ignore())
                .ForMember(dest => dest.IsActive,
                opt => opt.MapFrom(src => src.IsActive));

            CreateMap<UpdateCategoryAttributeRequest, CategoryAttribute>()
                .ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.DisplayName,
                opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.IsRequired,
                opt => opt.MapFrom(src => src.IsRequired))
                .ForMember(dest => dest.DisplayOrder,
                opt => opt.Ignore())
                .ForMember(dest => dest.Placeholder,
                opt => opt.MapFrom(src => src.Placeholder));

            CreateMap<UpdateAttributeOptionRequest, AttributeOption>()
                .ForMember(dest => dest.Value,
                opt => opt.MapFrom(src => src.Value))
                .ForMember(dest => dest.DisplayText,
                opt => opt.MapFrom(src => src.DisplayText));
        }
    }
}
