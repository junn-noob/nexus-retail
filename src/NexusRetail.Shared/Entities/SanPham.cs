using System;
using System.Collections.Generic;

namespace NexusRetail.Shared.Entities;

public partial class SanPham
{
    public string MaSp { get; set; } = null!;

    public string TenSp { get; set; } = null!;

    public string MaLoai { get; set; } = null!;

    public string MaNhanHieu { get; set; } = null!;

    public string MaManHinh { get; set; } = null!;

    public decimal GiaNhap { get; set; }

    public decimal GiaBan { get; set; }

    public int SoLuong { get; set; }

    public int ThoiGianBaoHanh { get; set; }

    public string? AmThanh { get; set; }

    public string? ChupAnh { get; set; }

    public string? Anh { get; set; }

    public string? GhiChu { get; set; }

    public virtual ICollection<ChatbotTuVanLog> ChatbotTuVanLogs { get; set; } = new List<ChatbotTuVanLog>();

    public virtual ICollection<ChiTietHdb> ChiTietHdbs { get; set; } = new List<ChiTietHdb>();

    public virtual ICollection<ChiTietHdn> ChiTietHdns { get; set; } = new List<ChiTietHdn>();

    public virtual Loai MaLoaiNavigation { get; set; } = null!;

    public virtual ManHinh MaManHinhNavigation { get; set; } = null!;

    public virtual NhanHieu MaNhanHieuNavigation { get; set; } = null!;
}
