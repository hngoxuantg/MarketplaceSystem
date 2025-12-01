using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Categories
{
    public class CreateCategoryViewModel
    {
        [Required(ErrorMessage = "Tên danh mục là bắt buộc!")]
        public required string Name { get; set; }

        public string? Description { get; set; }

        public int? ParentCategoryId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Thứ tự hiển thị phải lớn hơn 0!")]
        public int DisplayOrder { get; set; } = 1;

        public bool IsActive { get; set; } = true;

        // Icon file upload
        public IFormFile? IconFile { get; set; }

        public List<CreateCategoryAttributeDto>? Attributes { get; set; }
    }

    public class CreateCategoryAttributeDto
    {
        [Required(ErrorMessage = "Tên thuộc tính là bắt buộc!")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Tên hiển thị là bắt buộc!")]
        public required string DisplayName { get; set; }

        [Required(ErrorMessage = "Loại thuộc tính là bắt buộc!")]
        public required string AttributeType { get; set; } // "Text", "Number", "Select", "MultiSelect", "Boolean", "Date"

        public bool IsRequired { get; set; }

        [Required(ErrorMessage = "Thứ tự hiển thị là bắt buộc!")]
        [Range(1, int.MaxValue, ErrorMessage = "Thứ tự hiển thị phải lớn hơn 0!")]
        public int DisplayOrder { get; set; } = 1;

        public string? Placeholder { get; set; }

        public List<CreateAttributeOptionDto>? AttributeOptions { get; set; }
    }

    public class CreateAttributeOptionDto
    {
        [Required(ErrorMessage = "Giá trị là bắt buộc!")]
        public required string Value { get; set; }

        [Required(ErrorMessage = "Tên hiển thị là bắt buộc!")]
        public required string DisplayText { get; set; }
    }
}
