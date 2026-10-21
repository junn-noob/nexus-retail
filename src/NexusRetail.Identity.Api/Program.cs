using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NexusRetail.Identity.Api.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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

var jwtKey = builder.Configuration["Jwt:Key"] ?? "DuyMobileNexusRetailSecretKey2026SuperSecure!";
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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<AppIdentityDbContext>();
        await db.Database.EnsureCreatedAsync();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        string[] roles = { "Admin", "Staff", "Customer" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Default Admin
        if (await userManager.FindByNameAsync("admin") == null)
        {
            var adminUser = new IdentityUser
            {
                UserName = "admin",
                Email = "admin@duymobile.vn",
                EmailConfirmed = true,
                PhoneNumber = "0900000001"
            };
            var createRes = await userManager.CreateAsync(adminUser, "Admin@123");
            if (createRes.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Default Staff
        if (await userManager.FindByNameAsync("staff") == null)
        {
            var staffUser = new IdentityUser
            {
                UserName = "staff",
                Email = "staff@duymobile.vn",
                EmailConfirmed = true,
                PhoneNumber = "0900000002"
            };
            var createRes = await userManager.CreateAsync(staffUser, "Staff@123");
            if (createRes.Succeeded)
            {
                await userManager.AddToRoleAsync(staffUser, "Staff");
            }
        }

        // Default Customer
        if (await userManager.FindByNameAsync("customer") == null)
        {
            var custUser = new IdentityUser
            {
                UserName = "customer",
                Email = "customer@duymobile.vn",
                EmailConfirmed = true,
                PhoneNumber = "0912345678"
            };
            var createRes = await userManager.CreateAsync(custUser, "Cust@123");
            if (createRes.Succeeded)
            {
                await userManager.AddToRoleAsync(custUser, "Customer");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Lỗi khởi tạo CSDL Identity");
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
