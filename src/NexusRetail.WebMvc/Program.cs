using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<NexusRetail.WebMvc.Extensions.BearerTokenHandler>();

Action<HttpClient> configureCoreClient = client =>
{
    var baseUrl = builder.Configuration["ApiEndpoints:CoreApi"] ?? "https://localhost:7143/";
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
};

Func<HttpMessageHandler> createBypassHandler = () => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
};

builder.Services.AddHttpClient("CoreApi", configureCoreClient)
    .ConfigurePrimaryHttpMessageHandler(createBypassHandler)
    .AddHttpMessageHandler<NexusRetail.WebMvc.Extensions.BearerTokenHandler>();

builder.Services.AddHttpClient("IdentityApi", client =>
{
    var baseUrl = builder.Configuration["ApiEndpoints:IdentityApi"] ?? "http://localhost:5001/";
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.ConfigurePrimaryHttpMessageHandler(createBypassHandler);

builder.Services.AddHttpClient<NexusRetail.WebMvc.Services.Catalog.ICatalogApiClient, NexusRetail.WebMvc.Services.Catalog.CatalogApiClient>(configureCoreClient)
    .ConfigurePrimaryHttpMessageHandler(createBypassHandler)
    .AddHttpMessageHandler<NexusRetail.WebMvc.Extensions.BearerTokenHandler>();

builder.Services.AddHttpClient<NexusRetail.WebMvc.Services.Orders.IOrderApiClient, NexusRetail.WebMvc.Services.Orders.OrderApiClient>(configureCoreClient)
    .ConfigurePrimaryHttpMessageHandler(createBypassHandler)
    .AddHttpMessageHandler<NexusRetail.WebMvc.Extensions.BearerTokenHandler>();

builder.Services.AddHttpClient<NexusRetail.WebMvc.Services.Reports.IReportApiClient, NexusRetail.WebMvc.Services.Reports.ReportApiClient>(configureCoreClient)
    .ConfigurePrimaryHttpMessageHandler(createBypassHandler)
    .AddHttpMessageHandler<NexusRetail.WebMvc.Extensions.BearerTokenHandler>();

builder.Services.AddHttpClient<NexusRetail.WebMvc.Services.Admin.IAdminProductApiClient, NexusRetail.WebMvc.Services.Admin.AdminProductApiClient>(configureCoreClient)
    .ConfigurePrimaryHttpMessageHandler(createBypassHandler)
    .AddHttpMessageHandler<NexusRetail.WebMvc.Extensions.BearerTokenHandler>();

builder.Services.AddHttpClient<NexusRetail.WebMvc.Services.Admin.IAdminInboundApiClient, NexusRetail.WebMvc.Services.Admin.AdminInboundApiClient>(configureCoreClient)
    .ConfigurePrimaryHttpMessageHandler(createBypassHandler)
    .AddHttpMessageHandler<NexusRetail.WebMvc.Extensions.BearerTokenHandler>();

builder.Services.AddHttpClient<NexusRetail.WebMvc.Services.Admin.IAdminBrandApiClient, NexusRetail.WebMvc.Services.Admin.AdminBrandApiClient>(configureCoreClient)
    .ConfigurePrimaryHttpMessageHandler(createBypassHandler)
    .AddHttpMessageHandler<NexusRetail.WebMvc.Extensions.BearerTokenHandler>();

builder.Services.AddHttpClient<NexusRetail.WebMvc.Services.Chat.IChatApiClient, NexusRetail.WebMvc.Services.Chat.ChatApiClient>(configureCoreClient)
    .ConfigurePrimaryHttpMessageHandler(createBypassHandler)
    .AddHttpMessageHandler<NexusRetail.WebMvc.Extensions.BearerTokenHandler>();

builder.Services.AddHttpClient<NexusRetail.WebMvc.Services.Auth.IAuthApiClient, NexusRetail.WebMvc.Services.Auth.AuthApiClient>(client =>
{
    var baseUrl = builder.Configuration["ApiEndpoints:IdentityApi"] ?? "http://localhost:5001/";
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.ConfigurePrimaryHttpMessageHandler(createBypassHandler);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.Name = "NexusRetail.AuthCookie";
        options.Cookie.HttpOnly = true;
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
