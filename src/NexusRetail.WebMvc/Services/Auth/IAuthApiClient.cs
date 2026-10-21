using NexusRetail.Shared.DTOs.Auth;

namespace NexusRetail.WebMvc.Services.Auth
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "Customer";
        public string Token { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
    }

    public interface IAuthApiClient
    {
        Task<AuthResult> AuthenticateAsync(string username, string password);
        Task<(bool Success, string? ErrorMessage)> RegisterAsync(RegisterDto dto);
    }
}
