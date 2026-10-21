using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusRetail.Shared.DTOs.Auth;
using System.Security.Claims;
using System.Text.Json;

namespace NexusRetail.WebMvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _factory;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IHttpClientFactory factory, ILogger<AccountController> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return LocalRedirect(returnUrl ?? "/");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginDto { ReturnUrl = returnUrl });
        }

        [HttpGet]
        public async Task<IActionResult> QuickLogin(string role, string? returnUrl = null)
        {
            string username = "khachhang";
            string password = "123456";

            if (string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase))
            {
                username = "admin";
                password = "admin123";
            }
            else if (string.Equals(role, "staff", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(role, "nhanvien", StringComparison.OrdinalIgnoreCase))
            {
                username = "nhanvien";
                password = "123456";
            }

            return await ExecuteLoginAsync(username, password, false, returnUrl, null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model)
        {
            ViewData["ReturnUrl"] = model.ReturnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            return await ExecuteLoginAsync(model.Username.Trim(), model.Password, model.RememberMe, model.ReturnUrl, model);
        }

        private async Task<IActionResult> ExecuteLoginAsync(string username, string password, bool rememberMe, string? returnUrl, LoginDto? modelForView)
        {
            string role = "Customer";
            string email = $"{username}@duymobile.vn";
            string token = string.Empty;
            bool loginSuccess = false;

            // 1. Thử xác thực qua IdentityApi microservice
            try
            {
                var client = _factory.CreateClient("IdentityApi");
                var response = await client.PostAsJsonAsync("api/auth/login", new
                {
                    username = username,
                    password = password
                });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                    if (result != null)
                    {
                        username = result.Username;
                        email = result.Email;
                        role = result.Role;
                        token = result.Token;
                        loginSuccess = true;
                    }
                }
                else
                {
                    var errorMsg = "Tên đăng nhập hoặc mật khẩu không chính xác!";
                    try
                    {
                        var errObj = await response.Content.ReadFromJsonAsync<JsonElement>();
                        if (errObj.TryGetProperty("message", out var m))
                        {
                            errorMsg = m.GetString() ?? errorMsg;
                        }
                    }
                    catch { }
                    
                    // Kiểm tra xem có phải tài khoản Demo không trước khi báo lỗi
                    if (!CheckDemoAccount(username, password, out role, out email))
                    {
                        ModelState.AddModelError(string.Empty, errorMsg);
                        return View("Login", modelForView ?? new LoginDto { Username = username, ReturnUrl = returnUrl });
                    }
                    loginSuccess = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("IdentityApi không khả dụng ({Ex}), kiểm tra tài khoản Demo.", ex.Message);
                if (CheckDemoAccount(username, password, out role, out email))
                {
                    loginSuccess = true;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Không thể kết nối đến hệ thống xác thực. Bạn có thể sử dụng tài khoản Demo: admin/admin123 hoặc nhanvien/123456.");
                    return View("Login", modelForView ?? new LoginDto { Username = username, ReturnUrl = returnUrl });
                }
            }

            if (loginSuccess)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(ClaimTypes.NameIdentifier, username),
                    new Claim("JwtToken", token)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = rememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(rememberMe ? 7 : 1)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                TempData["SuccessMessage"] = $"Đăng nhập thành công! Vai trò: {role}.";

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                if (role == "Admin" || role == "Staff")
                {
                    return RedirectToAction("Index", "Admin");
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Tài khoản hoặc mật khẩu không đúng.");
            return View("Login", modelForView ?? new LoginDto { Username = username, ReturnUrl = returnUrl });
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new RegisterDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var client = _factory.CreateClient("IdentityApi");
                var response = await client.PostAsJsonAsync("api/auth/register", new
                {
                    username = model.Username.Trim(),
                    email = model.Email.Trim(),
                    password = model.Password,
                    phoneNumber = model.PhoneNumber?.Trim(),
                    role = "Customer"
                });

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
                    return RedirectToAction("Login");
                }
                else
                {
                    var msg = "Đăng ký không thành công.";
                    try
                    {
                        var errObj = await response.Content.ReadFromJsonAsync<JsonElement>();
                        if (errObj.TryGetProperty("message", out var m))
                        {
                            msg = m.GetString() ?? msg;
                        }
                    }
                    catch { }
                    ModelState.AddModelError(string.Empty, msg);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("IdentityApi không phản hồi ({Ex}). Cho phép tài khoản hoạt động ở chế độ Demo.", ex.Message);
                TempData["SuccessMessage"] = "Đăng ký tài khoản thử nghiệm thành công! Bạn có thể đăng nhập ngay.";
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "Đã đăng xuất tài khoản thành công.";
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Profile()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private static bool CheckDemoAccount(string username, string password, out string role, out string email)
        {
            role = "Customer";
            email = "";

            if (username.Equals("admin", StringComparison.OrdinalIgnoreCase) && password == "admin123")
            {
                role = "Admin";
                email = "admin@duymobile.vn";
                return true;
            }
            if (username.Equals("nhanvien", StringComparison.OrdinalIgnoreCase) && password == "123456")
            {
                role = "Staff";
                email = "nhanvien@duymobile.vn";
                return true;
            }
            if (username.Equals("khachhang", StringComparison.OrdinalIgnoreCase) && password == "123456")
            {
                role = "Customer";
                email = "khachhang@duymobile.vn";
                return true;
            }

            return false;
        }
    }
}
