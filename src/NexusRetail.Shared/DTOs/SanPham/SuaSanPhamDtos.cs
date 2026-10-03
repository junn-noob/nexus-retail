using System;
using System.Collections.Generic;
using System.Text;

namespace NexusRetail.Shared.DTOs.SanPham
{
    public class SuaSanPhamDtos
    {
        public string TenSp { get; set; } = null!;

        public string MaLoai { get; set; } = null!;

        public string MaNhanHieu { get; set; } = null!;

        public string MaManHinh { get; set; } = null!;

        public decimal GiaNhap { get; set; }

        public int SoLuong { get; set; }

        public int ThoiGianBaoHanh { get; set; }

        public string? AmThanh { get; set; }

        public string? ChupAnh { get; set; }

        public string? Anh { get; set; }

        public string? GhiChu { get; set; }
    }
}
