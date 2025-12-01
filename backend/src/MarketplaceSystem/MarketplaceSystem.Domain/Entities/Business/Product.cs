using MarketplaceSystem.Common.Enums;
using MarketplaceSystem.Domain.Entities.Base;
using MarketplaceSystem.Domain.Entities.Identity_Auth;
using MarketplaceSystem.Domain.Enums.Business;
using MarketplaceSystem.Infrastructure.Data.Repositories.BusinessRepositories;

namespace MarketplaceSystem.Domain.Entities.Business
{
    public class Product : SoftDeleteEntity
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public ProductCondition Condition { get; set; }
        public ProductStatus Status { get; set; }
        public int Quantity { get; set; }

        public VietnamProvince Location { get; set; }
        public int ViewCount { get; set; }

        public string? RejectionReason { get; private set; }
        public DateTime? RejectedAt { get; private set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public int? SellerId { get; private set; }
        public User? Seller { get; set; }

        public ProductClassification? ProductClassification { get; set; }


        private readonly List<ProductImage> _productImages = new List<ProductImage>();
        public IReadOnlyCollection<ProductImage> ProductImages => _productImages.AsReadOnly();


        private readonly List<ProductAttributeValue> _productAttributeValues = new List<ProductAttributeValue>();
        public virtual IReadOnlyCollection<ProductAttributeValue> ProductAttributeValues => _productAttributeValues.AsReadOnly();

        public Product() { }
        public Product(
            string title,
            string? description,
            decimal price,
            ProductCondition condition,
            int quantity,
            VietnamProvince location,
            int categoryId)
        {
            Title = title;
            Description = description;
            Price = price;
            Condition = condition;
            Quantity = quantity;
            Location = location;
            Status = ProductStatus.PendingApproval;
            CategoryId = categoryId;
        }
        public void AddAttributeValue(ProductAttributeValue attributeValue)
        {
            _productAttributeValues.Add(attributeValue);
        }

        public void SetSeller(int sellerId)
        {
            SellerId = sellerId;
        }

        public void AddImage(ProductImage image)
        {
            _productImages.Add(image);
        }

        public void Approve() => Status = ProductStatus.Active;

        public void Reject(string reason)
        {
            Status = ProductStatus.Rejected;
            RejectionReason = reason;
            RejectedAt = DateTime.UtcNow;
        }

        public void MarkAsSold()
        {
            Status = ProductStatus.Sold;
        }

        public void IncrementViewCount()
        {
            ViewCount++;
        }

        public void SetProductClassification(ContentWarningFlag flag, string? warningDetail)
        {
            ProductClassification = new ProductClassification(flag, warningDetail);
        }
    }
}
