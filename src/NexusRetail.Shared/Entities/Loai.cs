using System;
using System.Collections.Generic;

namespace NexusRetail.Shared.Entities;

public partial class Loai
{
    public string MaLoai { get; set; } = null!;

    public string TenLoai { get; set; } = null!;

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
