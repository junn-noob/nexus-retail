using System;
using System.Collections.Generic;
using System.Text;

namespace NexusRetail.Shared.DTOs.SanPham
{
    public class SanPhamDto
    {
        public string MaSp { get; set; } = null!;

        public string TenSp { get; set; } = null!;

        public string TenLoai { get; set; } = null!;

        public string MaNhanHieu { get; set; } = null!;

        public string TenNhanHieu { get; set; } = null!;

        public string TenManHinh { get; set; } = null!;

        public decimal GiaBan { get; set; }

        public int SoLuong { get; set; }

        public int ThoiGianBaoHanh { get; set; }

        public string? AmThanh { get; set; }

        public string? ChupAnh { get; set; }

        public string? Anh { get; set; }

        public string? GhiChu { get; set; }
    }
}
