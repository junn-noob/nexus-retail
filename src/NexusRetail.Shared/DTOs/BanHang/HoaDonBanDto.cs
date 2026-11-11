using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NexusRetail.Shared.DTOs.BanHang
{
    public class HoaDonBanDto
    {
        [StringLength(20, ErrorMessage = "Mã hóa đơn bán chỉ được nhập tối đa 20 ký tự")]
        public string? MaHdb { get; set; }

        [StringLength(20, ErrorMessage = "Mã nhân viên chỉ được nhập tối đa 20 ký tự")]
        public string? MaNv { get; set; }

        [StringLength(20, ErrorMessage = "Mã khách chỉ được nhập tối đa 20 ký tự")]
        public string? MaKhach { get; set; } = null!;

        public string? TenKhachVangLai { get; set; } = null!;

        [RegularExpression(@"^(03|05|07|08|09)\d{8}$", ErrorMessage = "Số điện thoại không hợp lệ!")]
        public string? DienThoai { get; set; } = null!;

        [StringLength(250, ErrorMessage = "Địa chỉ giao hàng tối đa 250 ký tự")]
        public string? DiaChiGiaoHang { get; set; }

        [StringLength(300, ErrorMessage = "Ghi chú tối đa 300 ký tự")]
        public string? GhiChu { get; set; }

        [StringLength(30, ErrorMessage = "Phương thức thanh toán tối đa 30 ký tự")]
        public string? PhuongThucThanhToan { get; set; }

        [Required(ErrorMessage = "Danh sách món không được để trống!")]
        [MinLength(1, ErrorMessage = "Hóa đơn phải có ít nhất 1 sản phẩm!")]
        public List<ChiTietHdbDto> DanhSachMon { get; set; } = null!;
    }
}
