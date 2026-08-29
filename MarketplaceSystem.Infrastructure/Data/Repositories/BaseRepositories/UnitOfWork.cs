using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBusinessRepositories;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IIdentity_AuthRepositories;
using MarketplaceSystem.Domain.Interfaces.IRepositories.ISystem_LogRepositories;
using MarketplaceSystem.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore.Storage;

namespace MarketplaceSystem.Infrastructure.Data.Repositories.BaseRepositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MarketplaceSystemDbContext _dbContext;
        private IDbContextTransaction? _transaction;
        public UnitOfWork(IRoleRepository roleRepository,
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            IAuditLogRepository auditLogRepository,
            IAttributeOptionRepository attributeOptionRepository,
            ICategoryAttributeRepository categoryAttributeRepository,
            ICategoryRepository categoryRepository,
            IProductAttributeValueRepository productAttributeValueRepository,
            IProductImageRepository productImageRepository,
            IProductRepository productRepository,
            IUserProfileRepository userProfileRepository,
            IFavoriteProductRepository favoriteProductRepository,
            IConversationRepository conversationRepository,
            IMessageRepository messageRepository,
            MarketplaceSystemDbContext dbContext)
        {
            RoleRepository = roleRepository;
            UserRepository = userRepository;
            RefreshTokenRepository = refreshTokenRepository;
            AuditLogRepository = auditLogRepository;
            AttributeOptionRepository = attributeOptionRepository;
            CategoryAttributeRepository = categoryAttributeRepository;
            CategoryRepository = categoryRepository;
            ProductAttributeValueRepository = productAttributeValueRepository;
            ProductImageRepository = productImageRepository;
            ProductRepository = productRepository;
            UserProfileRepository = userProfileRepository;
            FavoriteProductRepository = favoriteProductRepository;
            ConversationRepository = conversationRepository;
            MessageRepository = messageRepository;
            _dbContext = dbContext;
        }

        public IRoleRepository RoleRepository { get; }
        public IUserRepository UserRepository { get; }
        public IRefreshTokenRepository RefreshTokenRepository { get; }
        public IAuditLogRepository AuditLogRepository { get; }
        public IAttributeOptionRepository AttributeOptionRepository { get; }
        public ICategoryAttributeRepository CategoryAttributeRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        public IProductAttributeValueRepository ProductAttributeValueRepository { get; }
        public IProductImageRepository ProductImageRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IUserProfileRepository UserProfileRepository { get; }
        public IFavoriteProductRepository FavoriteProductRepository { get; set; }
        public IConversationRepository ConversationRepository { get; }
        public IMessageRepository MessageRepository { get; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }
        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
                _transaction.Dispose();
                _transaction = null;
            }
        }
        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _dbContext?.Dispose();
        }
    }
}
