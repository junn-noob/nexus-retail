using NexusRetail.Shared.DTOs.Auth;
using System.Text.Json;

namespace NexusRetail.WebMvc.Services.Auth
{
    public class AuthApiClient : IAuthApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<AuthApiClient> _logger;

        public AuthApiClient(HttpClient client, ILogger<AuthApiClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<AuthResult> AuthenticateAsync(string username, string password)
        {
            username = username.Trim();

            // 1. Thử gọi IdentityApi
            try
            {
                var response = await _client.PostAsJsonAsync("api/auth/login", new
                {
                    username,
                    password
                });

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                    if (result != null)
                    {
                        return new AuthResult
                        {
                            Success = true,
                            Username = result.Username,
                            Email = result.Email,
                            Role = result.Role,
                            Token = result.Token
                        };
                    }
                }
                else
                {
                    // Kiểm tra xem có phải tài khoản demo dự phòng không
                    if (CheckDemoAccount(username, password, out var demoRole, out var demoEmail))
                    {
                        return new AuthResult
                        {
                            Success = true,
                            Username = username,
                            Email = demoEmail,
                            Role = demoRole,
                            Token = "demo-local-token"
                        };
                    }

                    string errorMsg = "Tên đăng nhập hoặc mật khẩu không chính xác!";
                    try
                    {
                        var errObj = await response.Content.ReadFromJsonAsync<JsonElement>();
                        if (errObj.TryGetProperty("message", out var m))
                        {
                            errorMsg = m.GetString() ?? errorMsg;
                        }
                    }
                    catch { }

                    return new AuthResult
                    {
                        Success = false,
                        ErrorMessage = errorMsg
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("[AuthApiClient] IdentityApi không phản hồi ({Ex}). Kiểm tra tài khoản Demo dự phòng.", ex.Message);
            }

            // 2. Fallback sang Demo Account nếu IdentityApi không khả dụng
            if (CheckDemoAccount(username, password, out var role, out var email))
            {
                return new AuthResult
                {
                    Success = true,
                    Username = username,
                    Email = email,
                    Role = role,
                    Token = "demo-local-token"
                };
            }

            return new AuthResult
            {
                Success = false,
                ErrorMessage = "Tên đăng nhập hoặc mật khẩu không chính xác!"
            };
        }

        public async Task<(bool Success, string? ErrorMessage)> RegisterAsync(RegisterDto dto)
        {
            try
            {
                var response = await _client.PostAsJsonAsync("api/auth/register", new
                {
                    username = dto.Username.Trim(),
                    email = dto.Email.Trim(),
                    password = dto.Password,
                    phoneNumber = dto.PhoneNumber?.Trim(),
                    role = "Customer"
                });

                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }

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

                return (false, msg);
            }
            catch (Exception ex)
            {
                _logger.LogWarning("[AuthApiClient] IdentityApi không phản hồi ({Ex}). Cho phép tài khoản hoạt động ở chế độ Demo.", ex.Message);
                return (true, null);
            }
        }

        private static bool CheckDemoAccount(string username, string password, out string role, out string email)
        {
            role = "Customer";
            email = $"{username}@duymobile.vn";

            var u = username.Trim().ToLower();

            if (u == "admin" && password == "admin123")
            {
                role = "Admin";
                email = "admin@duymobile.vn";
                return true;
            }

            if ((u == "nhanvien" || u == "staff") && password == "123456")
            {
                role = "Staff";
                email = "nhanvien@duymobile.vn";
                return true;
            }

            if ((u == "khachhang" || u == "customer") && password == "123456")
            {
                role = "Customer";
                email = "khachhang@duymobile.vn";
                return true;
            }

            return false;
        }
    }
}
