using System;
using System.Collections.Generic;

namespace NexusRetail.Shared.Entities;

public partial class VwChiTietHoaDonBan
{
    public string MaHdb { get; set; } = null!;

    public DateTime NgayBan { get; set; }

    public string MaNv { get; set; } = null!;

    public string TenNv { get; set; } = null!;

    public string MaKhach { get; set; } = null!;

    public string TenKhach { get; set; } = null!;

    public string SdtKhach { get; set; } = null!;

    public string MaSp { get; set; } = null!;

    public string TenSp { get; set; } = null!;

    public int SoLuong { get; set; }

    public decimal DonGiaNiemYet { get; set; }

    public decimal KhuyenMai { get; set; }

    public decimal ThanhTien { get; set; }

    public decimal TongTienHoaDon { get; set; }
}
