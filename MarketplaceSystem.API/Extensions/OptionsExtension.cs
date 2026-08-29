using MarketplaceSystem.Common.Options;

namespace MarketplaceSystem.API.Extensions
{
    public static class OptionsExtension
    {
        public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(configuration.GetSection(AppSettings.SectionName));

            services.Configure<AdminAccount>(configuration.GetSection(AdminAccount.SectionName));

            services.Configure<GeminiAI>(configuration.GetSection(GeminiAI.SectionName));

            return services;
        }
    }
}
