using System.ComponentModel.DataAnnotations;

namespace MarketplaceSystem.Web.UI.Admin.Models.ViewModel.Categories
{
    public class UpdateCategoryViewModel
    {
        [Required(ErrorMessage = "Tên danh mục là bắt buộc!")]
        public required string Name { get; set; }

        public string? Description { get; set; }

        public int? ParentCategoryId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Thứ tự hiển thị phải lớn hơn 0!")]
        public int DisplayOrder { get; set; } = 1;

        public bool IsActive { get; set; } = true;
    }

    public class UpdateCategoryAttributeViewModel
    {
        [Required(ErrorMessage = "Tên thuộc tính là bắt buộc!")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Tên hiển thị là bắt buộc!")]
        public required string DisplayName { get; set; }

        public bool IsRequired { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Thứ tự hiển thị phải lớn hơn 0!")]
        public int DisplayOrder { get; set; } = 1;

        public string? Placeholder { get; set; }
    }

    public class CreateAttributeForCategoryViewModel
    {
        [Required(ErrorMessage = "Tên thuộc tính là bắt buộc!")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Tên hiển thị là bắt buộc!")]
        public required string DisplayName { get; set; }

        [Required(ErrorMessage = "Loại thuộc tính là bắt buộc!")]
        public required string AttributeType { get; set; } // "Text", "Number", "Select", "MultiSelect", "Boolean", "Date"

        public bool IsRequired { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Thứ tự hiển thị phải lớn hơn 0!")]
        public int DisplayOrder { get; set; } = 1;

        public string? Placeholder { get; set; }

        public List<CreateAttributeOptionForUpdateDto>? AttributeOptions { get; set; }
    }

    public class CreateAttributeOptionForUpdateDto
    {
        [Required(ErrorMessage = "Giá trị là bắt buộc!")]
        public required string Value { get; set; }

        [Required(ErrorMessage = "Tên hiển thị là bắt buộc!")]
        public required string DisplayText { get; set; }
    }

    public class UpdateAttributeOptionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Giá trị là bắt buộc!")]
        public required string Value { get; set; }

        [Required(ErrorMessage = "Tên hiển thị là bắt buộc!")]
        public required string DisplayText { get; set; }

        public bool IsActive { get; set; } = true;

        public int CategoryAttributeId { get; set; }
    }
}
