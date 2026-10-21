using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
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
                if (User.IsInRole("Admin"))
                    return RedirectToAction("Index", "Admin");
                return Redirect(returnUrl ?? "/");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string phone, string password, bool remember = false, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ tên đăng nhập/số điện thoại và mật khẩu!");
                return View();
            }

            var loginPayload = new
            {
                username = phone.Trim(),
                password = password
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(loginPayload), Encoding.UTF8, "application/json");

            // Thử gọi qua API Gateway trước, nếu Gateway chưa bật thì gọi thẳng Identity API
            HttpResponseMessage? response = null;
            try
            {
                var gatewayClient = _factory.CreateClient("Gateway");
                response = await gatewayClient.PostAsync("api/auth/login", jsonContent);
            }
            catch
            {
                // Fallback gọi trực tiếp Identity.Api
                try
                {
                    var idClient = _factory.CreateClient("IdentityApi");
                    var retryContent = new StringContent(JsonSerializer.Serialize(loginPayload), Encoding.UTF8, "application/json");
                    response = await idClient.PostAsync("api/auth/login", retryContent);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Không thể kết nối tới dịch vụ xác thực Identity.Api");
                    ModelState.AddModelError("", "Không thể kết nối đến máy chủ xác thực. Vui lòng kiểm tra dịch vụ Identity.Api!");
                    return View();
                }
            }

            if (response == null || !response.IsSuccessStatusCode)
            {
                string errMsg = "Tên đăng nhập / Số điện thoại hoặc mật khẩu không chính xác!";
                if (response != null)
                {
                    try
                    {
                        var body = await response.Content.ReadAsStringAsync();
                        using var doc = JsonDocument.Parse(body);
                        if (doc.RootElement.TryGetProperty("message", out var m))
                            errMsg = m.GetString() ?? errMsg;
                    }
                    catch { }
                }
                ModelState.AddModelError("", errMsg);
                return View();
            }

            // Đăng nhập thành công -> đọc token & claims
            var resultStr = await response.Content.ReadAsStringAsync();
            using var jsonDoc = JsonDocument.Parse(resultStr);
            var root = jsonDoc.RootElement;

            string token = root.GetProperty("token").GetString() ?? "";
            string username = root.GetProperty("username").GetString() ?? phone;
            string role = root.TryGetProperty("role", out var r) ? (r.GetString() ?? "Customer") : "Customer";
            string email = root.TryGetProperty("email", out var e) ? (e.GetString() ?? "") : "";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Email, email),
                new Claim("JwtToken", token)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProps = new AuthenticationProperties
            {
                IsPersistent = remember,
                ExpiresUtc = remember ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProps);

            if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Admin");
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string fullName, string phone, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin bắt buộc!");
                return View();
            }

            if (password != confirmPassword)
            {
                ModelState.AddModelError("", "Mật khẩu xác nhận không khớp!");
                return View();
            }

            if (password.Length < 6)
            {
                ModelState.AddModelError("", "Mật khẩu phải có độ dài tối thiểu 6 ký tự!");
                return View();
            }

            var registerPayload = new
            {
                username = phone.Trim(),
                email = $"{phone.Trim()}@duymobile.vn",
                phoneNumber = phone.Trim(),
                password = password,
                role = "Customer"
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(registerPayload), Encoding.UTF8, "application/json");

            HttpResponseMessage? response = null;
            try
            {
                var gatewayClient = _factory.CreateClient("Gateway");
                response = await gatewayClient.PostAsync("api/auth/register", jsonContent);
            }
            catch
            {
                try
                {
                    var idClient = _factory.CreateClient("IdentityApi");
                    var retryContent = new StringContent(JsonSerializer.Serialize(registerPayload), Encoding.UTF8, "application/json");
                    response = await idClient.PostAsync("api/auth/register", retryContent);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Không thể kết nối tới dịch vụ xác thực Identity.Api");
                    ModelState.AddModelError("", "Không thể kết nối đến máy chủ xác thực. Vui lòng thử lại sau!");
                    return View();
                }
            }

            if (response == null || !response.IsSuccessStatusCode)
            {
                string errMsg = "Đăng ký không thành công!";
                if (response != null)
                {
                    try
                    {
                        var body = await response.Content.ReadAsStringAsync();
                        using var doc = JsonDocument.Parse(body);
                        if (doc.RootElement.TryGetProperty("message", out var m))
                            errMsg = m.GetString() ?? errMsg;
                    }
                    catch { }
                }
                ModelState.AddModelError("", errMsg);
                return View();
            }

            // Tự động đăng nhập luôn sau khi đăng ký
            return await Login(phone, password, false, null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
