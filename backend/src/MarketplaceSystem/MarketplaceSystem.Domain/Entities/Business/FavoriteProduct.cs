using MarketplaceSystem.Domain.Entities.Identity_Auth;

namespace MarketplaceSystem.Domain.Entities.Business
{
    public class FavoriteProduct
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }

        public DateTime CreateAt { get; set; }

        public User? User { get; set; }
        public Product? Product { get; set; }

        public FavoriteProduct() { }
        public FavoriteProduct(int userId, int productId)
        {
            UserId = userId;
            ProductId = productId;
            CreateAt = DateTime.UtcNow;
        }
    }
}
