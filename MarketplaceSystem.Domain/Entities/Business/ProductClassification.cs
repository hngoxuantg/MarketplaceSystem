using MarketplaceSystem.Domain.Entities.Base;
using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Enums.Business;

namespace MarketplaceSystem.Infrastructure.Data.Repositories.BusinessRepositories
{
    public class ProductClassification : SoftDeleteEntity
    {
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public ContentWarningFlag WarningFlag { get; set; }
        public string? WarningDetail { get; set; }

        public ProductClassification() { }

        public ProductClassification(ContentWarningFlag warningFlag, string? warningDetail)
        {
            WarningFlag = warningFlag;
            WarningDetail = warningDetail;
        }
    }
}
