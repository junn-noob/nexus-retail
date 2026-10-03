using System;
using System.Collections.Generic;

namespace NexusRetail.Shared.Entities;

public partial class Que
{
    public string MaQue { get; set; } = null!;

    public string TenQue { get; set; } = null!;

    public virtual ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();
}
