using MarketplaceSystem.Domain.Entities.Business;
using MarketplaceSystem.Domain.Entities.System_Log;
using Microsoft.AspNetCore.Identity;

namespace MarketplaceSystem.Domain.Entities.Identity_Auth
{
    public class User : IdentityUser<int>
    {
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public int? CreatedBy { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UpdatedBy { get; set; }

        public DateTime? DeleteAt { get; set; }
        public int? DeleteBy { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime? LastLoginAt { get; private set; }


        private readonly List<RefreshToken> _refreshTokens = new List<RefreshToken>();
        public IReadOnlyCollection<RefreshToken>? RefreshTokens => _refreshTokens.AsReadOnly();


        private readonly List<AuditLog> _auditLogs = new List<AuditLog>();
        public IReadOnlyCollection<AuditLog>? AuditLogs => _auditLogs.AsReadOnly();


        private readonly List<FavoriteProduct> _favoriteProducts = new List<FavoriteProduct>();
        public IReadOnlyCollection<FavoriteProduct> FavoriteProducts => _favoriteProducts.AsReadOnly();


        private readonly List<Product> _products = new List<Product>();
        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();


        public virtual UserProfile? Profile { get; set; }

        public virtual void SetCreated(int? createBy)
        {
            CreateAt = DateTime.UtcNow;
            CreatedBy = createBy;
        }
        public virtual void SetUpdated(int? updateBy)
        {
            UpdateAt = DateTime.UtcNow;
            UpdatedBy = updateBy;
        }
        public virtual void SetDeleted(int? deleteBy)
        {
            IsDeleted = true;
            DeleteAt = DateTime.UtcNow;
            DeleteBy = deleteBy;
        }
        public virtual void UndoDelete()
        {
            IsDeleted = false;
            DeleteBy = null;
            DeleteAt = null;
        }
        public void AddFavorite(int productId)
        {
            if (_favoriteProducts.Any(f => f.ProductId == productId))
                return;

            _favoriteProducts.Add(new FavoriteProduct(Id, productId));
        }
        public void RemoveFavorite(int productId)
        {
            FavoriteProduct? favorite = _favoriteProducts
                .FirstOrDefault(fp => fp.ProductId == productId && fp.UserId == Id);

            if (favorite != null)
                _favoriteProducts.Remove(favorite);
        }

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }
    }
}
