using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NexusRetail.Identity.Api.Data;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityDb")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppIdentityDbContext>()
.AddDefaultTokenProviders();

var jwtKey = Environment.GetEnvironmentVariable("NEXUS_JWT_SECRET")
             ?? builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
{
    throw new InvalidOperationException(
        "Jwt:Key must be configured (>= 32 chars) via NEXUS_JWT_SECRET, appsettings.Development.json, or appsettings.Local.json.");
}
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "DuyMobile.Identity.Api";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "DuyMobile.Clients";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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

builder.Services.AddProblemDetails();

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

// Tự động khởi tạo Schema và Seed dữ liệu tài khoản quản trị vào cơ sở dữ liệu Identity
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppIdentityDbContext>();
        context.Database.EnsureCreated();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        string[] roles = ["Admin", "Staff", "Customer"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        await EnsureSeedUserAsync(userManager, "admin", "admin@duymobile.vn", "0981234567", "admin123", "Admin", app.Environment.IsDevelopment());
        await EnsureSeedUserAsync(userManager, "nhanvien", "nhanvien@duymobile.vn", "0978999888", "123456", "Staff", app.Environment.IsDevelopment());
        await EnsureSeedUserAsync(userManager, "khachhang", "khachhang@duymobile.vn", "0966554433", "123456", "Customer", app.Environment.IsDevelopment());
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Lỗi khởi tạo và seed dữ liệu AppIdentityDbContext.");
    }
}

app.Run();

static async Task EnsureSeedUserAsync(
    UserManager<IdentityUser> userManager,
    string username,
    string email,
    string phoneNumber,
    string password,
    string role,
    bool resetMismatchedPasswordInDev)
{
    var user = await userManager.FindByNameAsync(username);
    if (user == null)
    {
        user = new IdentityUser
        {
            UserName = username,
            Email = email,
            PhoneNumber = phoneNumber,
            EmailConfirmed = true
        };
        var createRes = await userManager.CreateAsync(user, password);
        if (!createRes.Succeeded)
        {
            return;
        }
    }
    else if (resetMismatchedPasswordInDev && !await userManager.CheckPasswordAsync(user, password))
    {
        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        await userManager.ResetPasswordAsync(user, resetToken, password);
    }

    if (!await userManager.IsInRoleAsync(user, role))
    {
        await userManager.AddToRoleAsync(user, role);
    }
}
