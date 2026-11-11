namespace NexusRetail.Shared.Entities;

/// <summary>
/// Lớp cơ sở trừu tượng cho mọi chứng từ giao dịch (Hóa đơn bán lẻ, Phiếu nhập kho).
/// Thể hiện tính Kế thừa (Inheritance) và Đa hình (Polymorphism) trong mô hình miền.
/// </summary>
public abstract class ChungTuBase
{
    public virtual string MaNv { get; set; } = null!;

    public virtual decimal TongTien { get; set; }

    public abstract string LoaiChungTu { get; }

    public virtual decimal TinhThueVat(decimal tyLe = 0.10m) => Math.Round(TongTien * tyLe, 0);
}

