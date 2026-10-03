using System;
using System.Collections.Generic;

namespace NexusRetail.Shared.Entities;

public partial class NhanVien
{
    public string MaNv { get; set; } = null!;

    public string TenNv { get; set; } = null!;

    public string? GioiTinh { get; set; }

    public DateOnly? NgaySinh { get; set; }

    public string? DienThoai { get; set; }

    public string? DiaChi { get; set; }

    public string? MaQue { get; set; }

    public virtual ICollection<HoaDonBan> HoaDonBans { get; set; } = new List<HoaDonBan>();

    public virtual ICollection<HoaDonNhap> HoaDonNhaps { get; set; } = new List<HoaDonNhap>();

    public virtual Que? MaQueNavigation { get; set; }
}
