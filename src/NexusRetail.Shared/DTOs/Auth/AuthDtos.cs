using System;
using System.ComponentModel.DataAnnotations;

namespace NexusRetail.Shared.DTOs.Auth
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập!")]
        [MinLength(3, ErrorMessage = "Tên đăng nhập tối thiểu 3 ký tự!")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; } = string.Empty;

        [Display(Name = "Họ và tên")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ email!")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng!")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu!")]
        [MinLength(6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự!")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp!")]
        [Display(Name = "Xác nhận mật khẩu")]
        public string? ConfirmPassword { get; set; }

        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$", ErrorMessage = "Số điện thoại không hợp lệ!")]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }

        public string Role { get; set; } = "Customer";
    }

    public class LoginDto
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập!")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu!")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Ghi nhớ đăng nhập")]
        public bool RememberMe { get; set; } = true;

        public string? ReturnUrl { get; set; }
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "Customer";
        public DateTime Expiration { get; set; }
    }
}
