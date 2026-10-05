using System;
using System.Collections.Generic;
using System.Text;

namespace NexusRetail.Shared.DTOs.BanHang
{
    public class HoaDonBanResponseDto
    {
        public string MaHdb { get; set; } = null!;

        public string MaNv { get; set; } = null!;

        public string TenNv { get; set; } = null!;

        public DateTime NgayBan { get; set; }

        public List<ChiTietHdbDto> SanPham { get; set; } = null!;
        public decimal TongTien { get; set; }
    }
}
