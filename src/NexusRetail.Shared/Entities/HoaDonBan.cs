using System;
using System.Collections.Generic;

namespace NexusRetail.Shared.Entities;

public partial class HoaDonBan : ChungTuBase
{
    public string MaHdb { get; set; } = null!;

    public override string? MaNv { get; set; }

    public DateTime NgayBan { get; set; }

    public string MaKhach { get; set; } = null!;

    public override decimal TongTien { get; set; }

    public string PhuongThucThanhToan { get; set; } = "COD";

    public string TrangThaiThanhToan { get; set; } = "DaThanhToan";

    public string TrangThaiDonHang { get; set; } = "HoanThanh";

    public string? DiaChiGiaoHang { get; set; }

    public string? GhiChu { get; set; }

    public string? MaGiaoDich { get; set; }

    public decimal SoTienDaThanhToan { get; set; }

    public override string LoaiChungTu => "Hóa đơn bán lẻ";

    public virtual ICollection<ChiTietHdb> ChiTietHdbs { get; set; } = new List<ChiTietHdb>();

    public virtual KhachHang MaKhachNavigation { get; set; } = null!;

    public virtual NhanVien? MaNvNavigation { get; set; }
}
