using System;
using System.ComponentModel.DataAnnotations;

namespace NexusRetail.Shared.DTOs.NhapHang
{
    public class ChiTietHdnDto
    {
        [Required(ErrorMessage = "Mã sản phẩm nhập không được để trống!")]
        [StringLength(20, ErrorMessage = "Mã sản phẩm chỉ được nhập tối đa 20 ký tự")]
        public string MaSp { get; set; } = null!;

        public string? TenSp { get; set; }

        [Required(ErrorMessage = "Số lượng nhập không được để trống!")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng nhập phải lớn hơn 0!")]
        public int SoLuong { get; set; }

        [Required(ErrorMessage = "Đơn giá nhập không được để trống!")]
        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá nhập phải lớn hơn hoặc bằng 0!")]
        public decimal DonGia { get; set; }

        [Range(0, 100, ErrorMessage = "Khuyến mại giảm giá phải từ 0% đến 100%!")]
        public decimal KhuyenMai { get; set; } = 0;

        public decimal ThanhTien { get; set; }
    }
}
