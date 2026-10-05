using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NexusRetail.Shared.DTOs.BanHang
{
    public class HoaDonBanDto
    {
        [Required]
        [StringLength(20, ErrorMessage = "Mã nhân viên chỉ được nhập tối đa 20 ký tự")]
        public string MaNv { get; set; } = null!;

        [StringLength(20, ErrorMessage = "Mã khách chỉ được nhập tối đa 20 ký tự")]
        public string? MaKhach { get; set; } = null!;

        public string? TenKhachVangLai { get; set; } = null!;

        [RegularExpression(@"^(03|05|07|08|09)\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ!")]
        public string? DienThoai { get; set; } = null!;

        [Required(ErrorMessage = "Danh sách món không được để trống!")]
        [MinLength(1, ErrorMessage = "Hóa đơn phải có ít nhất 1 sản phẩm!")]
        public List<ChiTietHdbDto> DanhSachMon { get; set; } = null!;
    }
}
