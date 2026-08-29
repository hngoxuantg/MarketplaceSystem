using FluentValidation;
using FluentValidation.AspNetCore;
using MarketplaceSystem.API.Extensions;
using MarketplaceSystem.API.Middlewares;
using MarketplaceSystem.Application.Common.Interfaces.IDataSeedingServices;
using MarketplaceSystem.Application.Common.Mappers;
using MarketplaceSystem.Application.Features.Auth.Commands.Login;
using MarketplaceSystem.Infrastructure.Data.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region Database
builder.Services.AddDbContext<MarketplaceSystemDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("PrimaryDbConnection"));
});
#endregion

#region Options
builder.Services.AddCustomOptions(builder.Configuration);
#endregion

#region Custom Services
builder.Services.Register();
builder.Services.RegisterSecurityService(builder.Configuration);
builder.Services.AddCustomCors(builder.Configuration);
builder.Services.AddCustomApiVersioning();
builder.Services.AddCustomSwagger();
builder.Services.AddCustomSignalR();
#endregion

#region Framework Services
builder.Services.AddCustomControllers();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization();
builder.Services.AddHttpClient();
#endregion

#region AutoMapper
builder.Services.AddAutoMapper(typeof(CategoryProfile).Assembly);
#endregion

#region MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly));
#endregion

#region Validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(LoginRequestValidator).Assembly);
builder.Services.AddCustomFluentValidation();
#endregion

var app = builder.Build();

#region Database Initialization
using var scope = app.Services.CreateScope();

var db = scope.ServiceProvider.GetRequiredService<MarketplaceSystemDbContext>();
await db.Database.MigrateAsync();

var seedingService = scope.ServiceProvider.GetRequiredService<IDataSeedingService>();
await seedingService.SeedDataAsync();
#endregion

#region Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DefaultModelsExpandDepth(-1);

        var groups = new[]
        {
            ("admin-v1", "Admin API - V1"),
            ("user-v1", "User API - V1"),
            ("public-v1", "Public API - V1")
        };

        foreach (var (groupName, displayName) in groups)
        {
            options.SwaggerEndpoint(
                $"/swagger/{groupName}/swagger.json",
                displayName);
        }

        options.EnableDeepLinking();
        options.DisplayRequestDuration();
        options.EnableFilter();
        options.ShowExtensions();
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors(CorsExtension.GetApiPolicyName());

#region Custom Middlewares
app.UseExceptionHandling();
app.UseRequestResponseLogging();
#endregion

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapCustomHubs();
#endregion

app.Run();
