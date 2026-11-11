using System;
using System.Collections.Generic;
using System.Text;

namespace NexusRetail.Shared.DTOs.BanHang
{
    public class HoaDonBanResponseDto
    {
        public string MaHdb { get; set; } = null!;

        public string? MaNv { get; set; }

        public string TenNv { get; set; } = null!;

        public DateTime NgayBan { get; set; }

        public List<ChiTietHdbDto> SanPham { get; set; } = null!;
        public decimal TongTien { get; set; }
        public string PhuongThucThanhToan { get; set; } = "COD";
        public string TrangThaiThanhToan { get; set; } = "ChuaThanhToan";
        public string TrangThaiDonHang { get; set; } = "ChoXacNhan";
        public string? DiaChiGiaoHang { get; set; }
        public string? GhiChu { get; set; }
    }
}
