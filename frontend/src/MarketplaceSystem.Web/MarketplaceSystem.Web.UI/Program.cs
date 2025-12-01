using MarketplaceSystem.Web.UI.Handlers;
using MarketplaceSystem.Web.UI.Interfaces;
using MarketplaceSystem.Web.UI.Interfaces.IBaseServices;
using MarketplaceSystem.Web.UI.Middlewares;
using MarketplaceSystem.Web.UI.Services;
using MarketplaceSystem.Web.UI.Services.BaseServices;

var builder = WebApplication.CreateBuilder(args);
Console.OutputEncoding = System.Text.Encoding.UTF8;

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

builder.Services.AddMemoryCache();

builder.Services.AddTransient<LoggingHandler>();
builder.Services.AddTransient<TokenHandler>();

builder.Services.AddHttpClient("ApiClients", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AppSettings:BaseUrl"]);
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
})
.AddHttpMessageHandler<LoggingHandler>()
.AddHttpMessageHandler<TokenHandler>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBaseApiService, BaseApiService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IEnumService, EnumService>();
builder.Services.AddScoped<IChatService, ChatService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "Cookies";
})
.AddCookie("Cookies", options =>
{
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(1);
    options.SlidingExpiration = true;
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

// Populate HttpContext.User from accessToken cookie (JWT) so [Authorize] works
app.UseAuthentication();

// Custom middleware: read JWT from accessToken cookie and set HttpContext.User
app.UseJwtCookieAuthentication();

// Populate HttpContext.User from accessToken cookie (JWT) after default authentication
// so that if the default authentication didn't produce a principal, we still allow [Authorize]
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
