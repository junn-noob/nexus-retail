using System.ComponentModel.DataAnnotations;

namespace NexusRetail.Identity.Api.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống!")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Email không được để trống!")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ!")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự!")]
        public string Password { get; set; } = "";

        public string? PhoneNumber { get; set; }

        public string Role { get; set; } = "Customer"; // Customer, Staff, Admin
    }

    public class LoginDto
    {
        [Required(ErrorMessage = "Tên đăng nhập không được để trống!")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Mật khẩu không được để trống!")]
        public string Password { get; set; } = "";
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = "";
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public DateTime Expiration { get; set; }
    }
}
