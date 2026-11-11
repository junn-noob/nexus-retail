using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NexusRetail.Core.Api.BUS;
using NexusRetail.Core.Api.Data;
using NexusRetail.Core.Api.Services.Chat;
using NexusRetail.Core.Api.Services.Export;
using NexusRetail.Core.Api.Services.Payment;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddHttpClient();

// Infrastructure & Data Access Layer (DAL)
builder.Services.AddDbContext<RetailDbContext>(x =>
    x.UseSqlServer(builder.Configuration.GetConnectionString("RetailDb")));
builder.Services.AddScoped<IAdoNetDataProvider, AdoNetDataProvider>();

// Domain Services & Business Logic Layer (BUS)
builder.Services.AddScoped<IExcelExportService, ExcelExportService>();
builder.Services.AddScoped<IGeminiChatService, GeminiChatService>();
builder.Services.AddSingleton<IBankPaymentService, BankPaymentService>();
builder.Services.AddScoped<ISanPhamBUS, SanPhamBUS>();
builder.Services.AddScoped<IBanHangBUS, BanHangBUS>();
builder.Services.AddScoped<INhapHangBUS, NhapHangBUS>();
builder.Services.AddScoped<IBaoCaoBUS, BaoCaoBUS>();

// Server-side JWT Authentication synchronized with Identity.Api
var jwtKey = Environment.GetEnvironmentVariable("NEXUS_JWT_SECRET")
             ?? builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
{
    throw new InvalidOperationException(
        "Jwt:Key must be configured (>= 32 chars) via NEXUS_JWT_SECRET, appsettings.Development.json, or appsettings.Local.json.");
}
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "DuyMobile.Identity.Api";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "DuyMobile.Clients";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
