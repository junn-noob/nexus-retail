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
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AuthApiClient] Lỗi kết nối đến dịch vụ xác thực IdentityApi.");
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = "Không thể kết nối đến máy chủ xác thực tài khoản. Vui lòng kiểm tra dịch vụ và thử lại!"
                };
            }
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
                _logger.LogError(ex, "[AuthApiClient] Lỗi kết nối đến dịch vụ IdentityApi khi đăng ký.");
                return (false, "Không thể kết nối đến máy chủ xác thực tài khoản. Vui lòng thử lại sau!");
            }
        }
    }
}
