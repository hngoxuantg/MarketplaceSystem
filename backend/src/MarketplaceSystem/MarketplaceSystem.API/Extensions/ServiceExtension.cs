using MarketplaceSystem.Application.Common.Interfaces.IDataSeedingServices;
using MarketplaceSystem.Application.Common.Interfaces.IExternalServices.IAIServices;
using MarketplaceSystem.Application.Common.Interfaces.IExternalServices.IMailServices;
using MarketplaceSystem.Application.Common.Interfaces.IExternalServices.IStorageServices;
using MarketplaceSystem.Application.Common.Interfaces.IExternalServices.ITokenServices;
using MarketplaceSystem.Application.Common.Interfaces.IServices;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBaseRepositories;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IBusinessRepositories;
using MarketplaceSystem.Domain.Interfaces.IRepositories.IIdentity_AuthRepositories;
using MarketplaceSystem.Domain.Interfaces.IRepositories.ISystem_LogRepositories;
using MarketplaceSystem.Infrastructure.Data.DataSeedingServices;
using MarketplaceSystem.Infrastructure.Data.Repositories.BaseRepositories;
using MarketplaceSystem.Infrastructure.Data.Repositories.BusinessRepositories;
using MarketplaceSystem.Infrastructure.Data.Repositories.Identity_AuthRepositories;
using MarketplaceSystem.Infrastructure.Data.Repositories.System_LogRepositories;
using MarketplaceSystem.Infrastructure.ExternalServices.IAIServices;
using MarketplaceSystem.Infrastructure.ExternalServices.MailServices;
using MarketplaceSystem.Infrastructure.ExternalServices.StorageServices;
using MarketplaceSystem.Infrastructure.ExternalServices.TokenServices;
using MarketplaceSystem.Infrastructure.Services;

namespace MarketplaceSystem.API.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection Register(this IServiceCollection services)
        {
            RegisterServices(services);
            RegisterRepositories(services);
            RegisterSeedData(services);
            return services;
        }
        public static IServiceCollection RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IGeminiAIService, GeminiAIService>();

            return services;
        }
        public static IServiceCollection RegisterRepositories(IServiceCollection services)
        {
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IAttributeOptionRepository, AttributeOptionRepository>();
            services.AddScoped<ICategoryAttributeRepository, CategoryAttributeRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductAttributeValueRepository, ProductAttributeValueRepository>();
            services.AddScoped<IProductImageRepository, ProductImageRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            services.AddScoped<IFavoriteProductRepository, FavoriteProductRepository>();
            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
        public static IServiceCollection RegisterSeedData(IServiceCollection services)
        {
            services.AddScoped<IDataSeedingService, DataSeedingService>();
            return services;
        }
        public static IServiceCollection RegisterValidator(IServiceCollection services)
        {
            return services;
        }
    }
}
