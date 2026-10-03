using System;
using System.Collections.Generic;

namespace NexusRetail.Shared.Entities;

public partial class VwDanhSachSanPham
{
    public string MaSp { get; set; } = null!;

    public string TenSp { get; set; } = null!;

    public string MaLoai { get; set; } = null!;

    public string TenLoai { get; set; } = null!;

    public string MaNhanHieu { get; set; } = null!;

    public string TenNhanHieu { get; set; } = null!;

    public string MaManHinh { get; set; } = null!;

    public string TenManHinh { get; set; } = null!;

    public decimal GiaNhap { get; set; }

    public decimal GiaBan { get; set; }

    public int SoLuong { get; set; }

    public int ThoiGianBaoHanh { get; set; }

    public string? AmThanh { get; set; }

    public string? ChupAnh { get; set; }

    public string? Anh { get; set; }

    public string? GhiChu { get; set; }
}
