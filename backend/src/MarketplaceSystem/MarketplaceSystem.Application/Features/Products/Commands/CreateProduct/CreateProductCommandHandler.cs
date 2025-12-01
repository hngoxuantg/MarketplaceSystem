using AutoMapper;
using MarketplaceSystem.Application.Common.DTOs.Products;
using MarketplaceSystem.Application.Common.Exceptions;
using MarketplaceSystem.Application.Common.Interfaces.IServices;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Entities.Identity_Auth;
using MarketplaceSystem.Domain.Enums.Business;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceSystem.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        public CreateProductCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<ProductDto> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            ProductDto productDto = await CreateProductAsync(command.Request, cancellationToken);
            return productDto;
        }

        private async Task<ProductDto> CreateProductAsync(CreateProductRequest request, CancellationToken cancellation = default)
        {
            Category? category = await _unitOfWork.CategoryRepository.GetOneUntrackedAsync<Category>(
                filter: c => c.Id == request.CategoryId && c.IsActive && !c.IsDeleted,
                include: c => c.Include(c => c.CategoryAttributes)
                    .ThenInclude(c => c.AttributeOptions),
                cancellation: cancellation
                ) ?? throw new NotFoundException("Danh mục không tồn tại");

            if (category.ParentCategoryId == null)
                throw new BusinessRuleException("Không được phép tạo sản phẩm cho danh mục cấp 1");

            await IsValidSellerAsync(_currentUserService.UserId ?? 0, cancellation);
            await IsValidProductAttributes(category.CategoryAttributes.ToList(), request.AttributeValues, cancellation);

            Product product = _mapper.Map<Product>(request);

            foreach (var attrDto in request.AttributeValues)
            {
                ProductAttributeValue attributeValue = _mapper.Map<ProductAttributeValue>(attrDto);
                product.AddAttributeValue(attributeValue);
            }

            product.SetSeller(_currentUserService.UserId ?? 0);

            await _unitOfWork.ProductRepository.CreateAsync(product, cancellation);

            ProductDto productDto = _mapper.Map<ProductDto>(product);

            return productDto;
        }
        private async Task IsValidSellerAsync(int sellerId, CancellationToken cancellation = default)
        {
            User user = await _unitOfWork.UserRepository.GetOneUntrackedAsync(
                filter: u => u.Id == sellerId,
                cancellation: cancellation,
                selector: u => new User
                {
                    IsDeleted = u.IsDeleted,
                    Profile = new UserProfile
                    {
                        SellerVerificationStatus = u.Profile.SellerVerificationStatus
                    }
                }) ?? throw new NotFoundException("Người bán không tồn tại");

            if (user.IsDeleted)
            {
                throw new BusinessRuleException("Người bán đã bị xóa!");
            }
        }

        private async Task IsValidProductAttributes(
            List<CategoryAttribute> attributes,
            List<CreateProductAttributeValueRequest> productAttr,
            CancellationToken cancellation = default)
        {
            var validationErrors = new Dictionary<string, List<string>>();

            var requiredAttributes = attributes.Where(a => a.IsRequired).ToList();

            for (int i = 0; i < requiredAttributes.Count; i++)
            {
                var reqAttr = requiredAttributes[i];

                string attributePath = reqAttr.AttributeType switch
                {
                    AttributeType.Text => $"{nameof(CreateProductRequest.AttributeValues)}[{i}].{nameof(CreateProductAttributeValueRequest.TextValue)}",
                    AttributeType.Number => $"{nameof(CreateProductRequest.AttributeValues)}[{i}].{nameof(CreateProductAttributeValueRequest.NumberValue)}",
                    AttributeType.Date => $"{nameof(CreateProductRequest.AttributeValues)}[{i}].{nameof(CreateProductAttributeValueRequest.DateValue)}",
                    AttributeType.Boolean => $"{nameof(CreateProductRequest.AttributeValues)}[{i}].{nameof(CreateProductAttributeValueRequest.BooleanValue)}",
                    AttributeType.Select => $"{nameof(CreateProductRequest.AttributeValues)}[{i}].{nameof(CreateProductAttributeValueRequest.SelectValues)}",
                    _ => $"{nameof(CreateProductRequest.AttributeValues)}[{i}]"
                };

                var provided = productAttr
                    .Where(a => a.CategoryAttributeId == reqAttr.Id)
                    .ToList();

                if (!provided.Any() || !provided.Any(a => a.HasValue()))
                {
                    validationErrors[attributePath] = new List<string>
                    {
                        $"Thuộc tính '{reqAttr.DisplayName}' là bắt buộc!"
                    };
                }
            }

            if (validationErrors.Any())
                throw new ValidatorException(validationErrors);

            foreach (var attrDto in productAttr)
            {
                var categoryAttribute = attributes
                    .FirstOrDefault(a => a.Id == attrDto.CategoryAttributeId)
                    ?? throw new BusinessRuleException($"Thuộc tính không hợp lệ với danh mục sản phẩm: {attrDto.CategoryAttributeId}");

                switch (categoryAttribute.AttributeType)
                {
                    case AttributeType.Text:
                        ValidateTextAttribute(attrDto);
                        break;
                    case AttributeType.Number:
                        ValidateNumberAttribute(attrDto);
                        break;
                    case AttributeType.Date:
                        ValidateDateAttribute(attrDto);
                        break;
                    case AttributeType.Boolean:
                        ValidateBooleanAttribute(attrDto);
                        break;
                    case AttributeType.Select:
                        ValidateSelectAttribute(attrDto, categoryAttribute.AttributeOptions?.ToList());
                        break;
                }
            }
        }


        private void ValidateTextAttribute(CreateProductAttributeValueRequest attributeValueRequest)
        {
            if (string.IsNullOrEmpty(attributeValueRequest.TextValue))
                throw new ValidatorException(nameof(CreateProductAttributeValueRequest.TextValue), "Không được để trống!");

            if (attributeValueRequest.TextValue.Length > 100)
                throw new ValidatorException(nameof(CreateProductAttributeValueRequest.TextValue), "Không được quá 100 kí tự!");

            if (attributeValueRequest.NumberValue.HasValue ||
                attributeValueRequest.DateValue.HasValue ||
                attributeValueRequest.BooleanValue.HasValue ||
                !string.IsNullOrEmpty(attributeValueRequest.SelectValues))
            {
                throw new BusinessRuleException($"Thuộc tính '{attributeValueRequest.TextValue}' chỉ nhận giá trị văn bản!");
            }
        }
        private void ValidateNumberAttribute(CreateProductAttributeValueRequest attributeValueRequest)
        {
            if (!attributeValueRequest.NumberValue.HasValue)
                throw new ValidatorException(nameof(CreateProductAttributeValueRequest.NumberValue), "Không được để trống!");

            if (attributeValueRequest.NumberValue <= 0)
                throw new ValidatorException(nameof(CreateProductAttributeValueRequest.NumberValue), "Giá trị phải lớn hơn 0!");

            if (!string.IsNullOrEmpty(attributeValueRequest.TextValue) ||
                attributeValueRequest.DateValue.HasValue ||
                attributeValueRequest.BooleanValue.HasValue ||
                !string.IsNullOrEmpty(attributeValueRequest.SelectValues))
            {
                throw new BusinessRuleException($"Thuộc tính '{attributeValueRequest.NumberValue}' chỉ nhận giá trị số!");
            }
        }
        private void ValidateBooleanAttribute(CreateProductAttributeValueRequest attributeValueRequest)
        {
            if (!attributeValueRequest.BooleanValue.HasValue)
                throw new ValidatorException(nameof(CreateProductAttributeValueRequest.BooleanValue), "Không được để trống!");

            if (!string.IsNullOrEmpty(attributeValueRequest.TextValue) ||
                attributeValueRequest.DateValue.HasValue ||
                attributeValueRequest.NumberValue.HasValue ||
                !string.IsNullOrEmpty(attributeValueRequest.SelectValues))
            {
                throw new BusinessRuleException($"Thuộc tính '{attributeValueRequest.BooleanValue}' chỉ nhận giá boolean!");
            }
        }

        private void ValidateDateAttribute(CreateProductAttributeValueRequest attributeValueRequest)
        {
            if (!attributeValueRequest.DateValue.HasValue)
            {
                throw new ValidatorException(nameof(CreateProductAttributeValueRequest.DateValue),
                    $"Thuộc tính '{nameof(CreateProductAttributeValueRequest.DateValue)}' phải là dạng ngày tháng");
            }
            if (attributeValueRequest.DateValue.Value > DateTime.UtcNow.AddYears(10))
            {
                throw new BusinessRuleException(
                    $"Ngày không hợp lệ");
            }
            if (!string.IsNullOrWhiteSpace(attributeValueRequest.TextValue) ||
                attributeValueRequest.NumberValue.HasValue ||
                attributeValueRequest.BooleanValue.HasValue ||
                !string.IsNullOrEmpty(attributeValueRequest.SelectValues))
            {
                throw new BusinessRuleException(
                    $"Thuộc tính chỉ nhận giá trị ngày tháng");
            }
        }

        private void ValidateSelectAttribute(
            CreateProductAttributeValueRequest attributeValueRequest,
            List<AttributeOption>? attributeOptions)
        {
            if (string.IsNullOrWhiteSpace(attributeValueRequest.SelectValues))
            {
                throw new ValidatorException(
                    nameof(CreateProductAttributeValueRequest.SelectValues),
                    "Thuộc tính là bắt buộc!");
            }

            if (attributeOptions == null || !attributeOptions.Any())
            {
                throw new BusinessRuleException("Thuộc tính chưa được cấu hình giá trị!");
            }

            var allowedValues = attributeOptions
                .Select(ao => ao.Value)
                .ToList();

            var selectedValues = attributeValueRequest.SelectValues
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(v => v.Trim())
                .ToList();

            foreach (var selected in selectedValues)
            {
                if (!allowedValues.Contains(selected, StringComparer.OrdinalIgnoreCase))
                {
                    throw new BusinessRuleException(
                        $"Giá trị '{selected}' không hợp lệ. " +
                        $"Các giá trị được phép: {string.Join(", ", allowedValues)}");
                }
            }

            if (!string.IsNullOrEmpty(attributeValueRequest.TextValue) ||
                attributeValueRequest.NumberValue.HasValue ||
                attributeValueRequest.BooleanValue.HasValue ||
                attributeValueRequest.DateValue.HasValue)
            {
                throw new BusinessRuleException("Thuộc tính chỉ nhận giá trị lựa chọn!");
            }
        }
    }
}
