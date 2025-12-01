using MarketplaceSystem.Domain.Interfaces.IRepositories.IBusinessRepositories;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IIdentity_AuthRepositories;
using MarketplaceSystem.Domain.Interfaces.IRepositories.ISystem_LogRepositories;

namespace MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories
{
    public interface IUnitOfWork
    {
        IRoleRepository RoleRepository { get; }
        IUserRepository UserRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        IAuditLogRepository AuditLogRepository { get; }
        IAttributeOptionRepository AttributeOptionRepository { get; }
        ICategoryAttributeRepository CategoryAttributeRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IProductAttributeValueRepository ProductAttributeValueRepository { get; }
        IProductImageRepository ProductImageRepository { get; }
        IProductRepository ProductRepository { get; }
        IUserProfileRepository UserProfileRepository { get; }
        IFavoriteProductRepository FavoriteProductRepository { get; }
        IConversationRepository ConversationRepository { get; }
        IMessageRepository MessageRepository { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
        void Dispose();
    }
}
