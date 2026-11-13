using System;
using System.Collections.Generic;

namespace NexusRetail.Shared.Entities;

public partial class SanPhamMauSac
{
    public string MaSp { get; set; } = null!;

    public string MaMau { get; set; } = null!;

    public decimal GiaNhap { get; set; }

    public decimal GiaBan { get; set; }

    public int SoLuong { get; set; }

    public string? HinhAnh { get; set; }

    public virtual SanPham MaSpNavigation { get; set; } = null!;

    public virtual MauSac MaMauNavigation { get; set; } = null!;
}

