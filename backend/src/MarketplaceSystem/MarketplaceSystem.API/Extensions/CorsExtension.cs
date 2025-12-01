namespace MarketplaceSystem.API.Extensions
{
    public static class CorsExtension
    {
        private const string ApiCorsPolicy = "ApiCorsPolicy";
        private const string HubCorsPolicy = "HubCorsPolicy";
        public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration configuration)
        {
            string[]? apiOrigins = configuration.GetSection("AllowedCors:ApiOrigins").Get<string[]>();
            string[]? hubOrigins = configuration.GetSection("AllowedCors:HubOrigins").Get<string[]>();

            services.AddCors(options =>
            {
                options.AddPolicy(ApiCorsPolicy, policy =>
                {
                    if (apiOrigins != null && apiOrigins.Length > 0)
                    {
                        policy.WithOrigins(apiOrigins)
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    }
                    else
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    }
                });

                options.AddPolicy(HubCorsPolicy, policy =>
                {
                    if (hubOrigins != null && hubOrigins.Length > 0)
                    {
                        policy.WithOrigins(hubOrigins)
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    }
                    else
                    {
                        policy.SetIsOriginAllowed(_ => true)
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    }
                });
            });

            return services;
        }


        public static string GetApiPolicyName() => ApiCorsPolicy;
        public static string GetHubPolicyName() => HubCorsPolicy;
    }
}
