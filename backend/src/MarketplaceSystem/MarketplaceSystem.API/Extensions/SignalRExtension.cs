using MarketplaceSystem.API.Hubs;

namespace MarketplaceSystem.API.Extensions
{
    public static class SignalRExtension
    {
        private static readonly string HubCorsPolicy = CorsExtension.GetHubPolicyName();

        public static IServiceCollection AddCustomSignalR(this IServiceCollection services)
        {
            services.AddSignalR();
            return services;
        }

        public static IEndpointRouteBuilder MapCustomHubs(this IEndpointRouteBuilder app)
        {
            app.MapHub<NotificationHub>("/hubs/notifications")
               .RequireCors(HubCorsPolicy);

            app.MapHub<ChatHub>("/hubs/chat")
               .RequireCors(HubCorsPolicy);

            return app;
        }
    }
}
