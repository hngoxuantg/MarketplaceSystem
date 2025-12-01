using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MarketplaceSystem.API.Extensions
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            var groups = new[] { "admin-v1", "user-v1", "public-v1" };

            foreach (var group in groups)
            {
                options.SwaggerDoc(group, CreateInfoForGroup(group));
            }
        }

        private static OpenApiInfo CreateInfoForGroup(string groupName)
        {
            var parts = groupName.Split('-');
            var role = parts[0];
            var version = parts[1].ToUpper();

            var info = new OpenApiInfo
            {
                Title = role switch
                {
                    "admin" => "Marketplace System - Admin API",
                    "user" => "Marketplace System - User API",
                    "public" => "Marketplace System - Public API",
                    _ => "Marketplace System API"
                },
                Version = version,
                Description = role switch
                {
                    "admin" => "Administrative operations for system management",
                    "user" => "Operations for authenticated users",
                    "public" => "Public operations available to all clients",
                    _ => string.Empty
                },
                Contact = new OpenApiContact
                {
                    Name = "Ngo Xuan Hai",
                    Email = "hngoxuantg.dev@gmail.com"
                }
            };

            return info;
        }
    }

    public static class SwaggerExtension
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. " +
                                  "Enter 'Bearer' [space] and then your token in the text input below. " +
                                  "Example: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "Bearer",
                            Name = "Authorization",
                            In = ParameterLocation.Header,
                        },
                        Array.Empty<string>()
                    }
                });

                options.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (apiDesc.ActionDescriptor is not ControllerActionDescriptor controllerDesc)
                        return false;

                    var controllerNamespace = controllerDesc.ControllerTypeInfo.Namespace ?? "";

                    if (docName == "admin-v1" && controllerNamespace.Contains(".Admin.V1"))
                        return true;

                    if (docName == "user-v1" && controllerNamespace.Contains(".User.V1"))
                        return true;

                    if (docName == "public-v1" && controllerNamespace.Contains(".Public.V1"))
                        return true;

                    return false;
                });

                options.OperationFilter<SwaggerOperationFilter>();
                options.CustomSchemaIds(type => type.FullName);
            });

            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            return services;
        }
    }

    public class SwaggerOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.ActionDescriptor is ControllerActionDescriptor controllerDesc)
            {
                var controllerName = controllerDesc.ControllerName;
                var namespaceName = controllerDesc.ControllerTypeInfo.Namespace ?? "";

                string prefix = namespaceName.Contains(".Admin") ? "Admin" :
                               namespaceName.Contains(".User") ? "User" : "Public";

                operation.Tags = new List<OpenApiTag>
                {
                    new OpenApiTag { Name = $"{prefix} - {controllerName}" }
                };
            }
        }
    }
}
