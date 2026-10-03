using System;
using System.Collections.Generic;

namespace NexusRetail.Shared.Entities;

public partial class ChiTietHdn
{
    public string MaHdn { get; set; } = null!;

    public string MaSp { get; set; } = null!;

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public decimal KhuyenMai { get; set; }

    public decimal ThanhTien { get; set; }

    public virtual HoaDonNhap MaHdnNavigation { get; set; } = null!;

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
