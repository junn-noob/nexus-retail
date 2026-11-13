namespace NexusRetail.Shared.DTOs.SanPham;

public class SanPhamMauSacDto
{
    public string MaSp { get; set; } = null!;
    public string MaMau { get; set; } = null!;
    public string TenMau { get; set; } = null!;
    public string? MaHex { get; set; }
    public decimal GiaNhap { get; set; }
    public decimal GiaBan { get; set; }
    public int SoLuong { get; set; }
    public string? HinhAnh { get; set; }
}

public class MauSacDto
{
    public string MaMau { get; set; } = null!;
    public string TenMau { get; set; } = null!;
    public string? MaHex { get; set; }
}
