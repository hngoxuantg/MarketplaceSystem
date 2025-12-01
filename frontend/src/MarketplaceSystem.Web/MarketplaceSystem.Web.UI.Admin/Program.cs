using MarketplaceSystem.Web.UI.Admin.Handlers;
using MarketplaceSystem.Web.UI.Admin.Interfaces;
using MarketplaceSystem.Web.UI.Admin.Interfaces.IBaseServices;
using MarketplaceSystem.Web.UI.Admin.Middlewares;
using MarketplaceSystem.Web.UI.Admin.Services;
using MarketplaceSystem.Web.UI.Admin.Services.BaseServices;

var builder = WebApplication.CreateBuilder(args);
Console.OutputEncoding = System.Text.Encoding.UTF8;

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

builder.Services.AddMemoryCache();

builder.Services.AddTransient<LoggingHandler>();
builder.Services.AddTransient<TokenHandler>();

builder.Services.AddHttpClient("ApiClients", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AppSettings:BaseUrl"] ?? "https://localhost:7079/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
})
.AddHttpMessageHandler<LoggingHandler>()
.AddHttpMessageHandler<TokenHandler>();

builder.Services.AddScoped<IBaseApiService, BaseApiService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEnumService, EnumService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandling();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();

app.UseAuthorization();
app.UseJwtCookieAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();