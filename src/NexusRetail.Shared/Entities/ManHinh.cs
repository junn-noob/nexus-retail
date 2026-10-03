using System;
using System.Collections.Generic;

namespace NexusRetail.Shared.Entities;

public partial class ManHinh
{
    public string MaManHinh { get; set; } = null!;

    public string TenManHinh { get; set; } = null!;

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
