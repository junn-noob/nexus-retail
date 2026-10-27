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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
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

        // 1. Quản trị viên (Admin)
        var adminUser = await userManager.FindByNameAsync("admin");
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = "admin",
                Email = "admin@duymobile.vn",
                EmailConfirmed = true
            };
            var res = await userManager.CreateAsync(adminUser, "admin123");
            if (res.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
        else if (!await userManager.CheckPasswordAsync(adminUser, "admin123"))
        {
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(adminUser);
            await userManager.ResetPasswordAsync(adminUser, resetToken, "admin123");
        }

        // 2. Nhân viên bán hàng & kho (Staff)
        var staffUser = await userManager.FindByNameAsync("nhanvien");
        if (staffUser == null)
        {
            staffUser = new IdentityUser
            {
                UserName = "nhanvien",
                Email = "nhanvien@duymobile.vn",
                EmailConfirmed = true
            };
            var res = await userManager.CreateAsync(staffUser, "123456");
            if (res.Succeeded)
            {
                await userManager.AddToRoleAsync(staffUser, "Staff");
            }
        }

        // 3. Khách hàng mẫu (Customer)
        var custUser = await userManager.FindByNameAsync("khachhang");
        if (custUser == null)
        {
            custUser = new IdentityUser
            {
                UserName = "khachhang",
                Email = "khachhang@duymobile.vn",
                EmailConfirmed = true
            };
            var res = await userManager.CreateAsync(custUser, "123456");
            if (res.Succeeded)
            {
                await userManager.AddToRoleAsync(custUser, "Customer");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Lỗi khởi tạo và seed dữ liệu AppIdentityDbContext.");
    }
}

app.Run();
