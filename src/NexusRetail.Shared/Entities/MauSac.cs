using System;
using System.Collections.Generic;

namespace NexusRetail.Shared.Entities;

public partial class MauSac
{
    public string MaMau { get; set; } = null!;

    public string TenMau { get; set; } = null!;

    public string? MaHex { get; set; }

    public virtual ICollection<SanPhamMauSac> SanPhamMauSacs { get; set; } = new List<SanPhamMauSac>();

    public virtual ICollection<ChiTietHdb> ChiTietHdbs { get; set; } = new List<ChiTietHdb>();

    public virtual ICollection<ChiTietHdn> ChiTietHdns { get; set; } = new List<ChiTietHdn>();
}
