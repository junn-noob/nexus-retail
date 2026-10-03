using System;
using System.Collections.Generic;

namespace NexusRetail.Shared.Entities;

public partial class NhaCungCap
{
    public string MaNcc { get; set; } = null!;

    public string TenNcc { get; set; } = null!;

    public string? DiaChi { get; set; }

    public string? DienThoai { get; set; }

    public virtual ICollection<HoaDonNhap> HoaDonNhaps { get; set; } = new List<HoaDonNhap>();
}
