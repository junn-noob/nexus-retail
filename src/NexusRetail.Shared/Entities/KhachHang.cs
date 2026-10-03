using System;
using System.Collections.Generic;

namespace NexusRetail.Shared.Entities;

public partial class KhachHang
{
    public string MaKhach { get; set; } = null!;

    public string TenKhach { get; set; } = null!;

    public string? DiaChi { get; set; }

    public string DienThoai { get; set; } = null!;

    public virtual ICollection<HoaDonBan> HoaDonBans { get; set; } = new List<HoaDonBan>();
}
