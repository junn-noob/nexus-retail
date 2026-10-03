using System;
using System.Collections.Generic;

namespace NexusRetail.Shared.Entities;

public partial class NhanHieu
{
    public string MaNhanHieu { get; set; } = null!;

    public string TenNhanHieu { get; set; } = null!;

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
