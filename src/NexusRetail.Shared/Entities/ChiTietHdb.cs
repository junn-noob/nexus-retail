using System;
using System.Collections.Generic;

namespace NexusRetail.Shared.Entities;

public partial class ChiTietHdb
{
    public string MaHdb { get; set; } = null!;

    public string MaSp { get; set; } = null!;

    public int SoLuong { get; set; }

    public decimal KhuyenMai { get; set; }

    public decimal ThanhTien { get; set; }

    public virtual HoaDonBan MaHdbNavigation { get; set; } = null!;

    public virtual SanPham MaSpNavigation { get; set; } = null!;
}
