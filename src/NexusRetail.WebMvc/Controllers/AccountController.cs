using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusRetail.Shared.DTOs.Auth;
using NexusRetail.WebMvc.Services.Auth;
using System.Security.Claims;

namespace NexusRetail.WebMvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthApiClient _authClient;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthApiClient authClient, ILogger<AccountController> logger)
        {
            _authClient = authClient;
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
            var authResult = await _authClient.AuthenticateAsync(username, password);

            if (authResult.Success)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, authResult.Username),
                    new Claim(ClaimTypes.Email, authResult.Email),
                    new Claim(ClaimTypes.Role, authResult.Role),
                    new Claim(ClaimTypes.NameIdentifier, authResult.Username),
                    new Claim("JwtToken", authResult.Token)
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

                TempData["SuccessMessage"] = $"Đăng nhập thành công! Vai trò: {authResult.Role}.";

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                if (authResult.Role == "Admin" || authResult.Role == "Staff")
                {
                    return RedirectToAction("Index", "Admin");
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, authResult.ErrorMessage ?? "Tài khoản hoặc mật khẩu không đúng.");
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

            var (success, errorMessage) = await _authClient.RegisterAsync(model);
            if (success)
            {
                TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError(string.Empty, errorMessage ?? "Đăng ký không thành công.");
            return View(model);
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
    }
}
