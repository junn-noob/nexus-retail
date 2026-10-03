using System;
using System.Collections.Generic;

namespace NexusRetail.Shared.Entities;

public partial class HoaDonBan
{
    public string MaHdb { get; set; } = null!;

    public string MaNv { get; set; } = null!;

    public DateTime NgayBan { get; set; }

    public string MaKhach { get; set; } = null!;

    public decimal TongTien { get; set; }

    public virtual ICollection<ChiTietHdb> ChiTietHdbs { get; set; } = new List<ChiTietHdb>();

    public virtual KhachHang MaKhachNavigation { get; set; } = null!;

    public virtual NhanVien MaNvNavigation { get; set; } = null!;
}
