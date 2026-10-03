using System;
using System.Collections.Generic;

namespace NexusRetail.Shared.Entities;

public partial class HoaDonNhap
{
    public string MaHdn { get; set; } = null!;

    public string MaNv { get; set; } = null!;

    public DateTime NgayNhap { get; set; }

    public string MaNcc { get; set; } = null!;

    public decimal TongTien { get; set; }

    public virtual ICollection<ChiTietHdn> ChiTietHdns { get; set; } = new List<ChiTietHdn>();

    public virtual NhaCungCap MaNccNavigation { get; set; } = null!;

    public virtual NhanVien MaNvNavigation { get; set; } = null!;
}
