using System;
using System.Collections.Generic;

namespace NexusRetail.Shared.DTOs.NhapHang
{
    public class HoaDonNhapResponseDto
    {
        public string MaHdn { get; set; } = null!;
        public string MaNv { get; set; } = null!;
        public string TenNv { get; set; } = "";
        public string MaNcc { get; set; } = null!;
        public string TenNcc { get; set; } = "";
        public DateTime NgayNhap { get; set; }
        public decimal TongTien { get; set; }
        public List<ChiTietHdnDto> SanPham { get; set; } = new();
    }
}
