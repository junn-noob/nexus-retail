namespace NexusRetail.WinForms.Services;

public class PaymentCheckResult
{
    public string MaHdb { get; set; } = "";
    public bool IsPaid { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal ExpectedAmount { get; set; }
    public string? TransactionCode { get; set; }
    public DateTime? PaidAt { get; set; }
    public string Message { get; set; } = "";
}

public class NextCodeResult
{
    public string MaHdb { get; set; } = "";
}

public class NextInboundCodeResult
{
    public string MaHdn { get; set; } = "";
}

public class NhaCungCapItem
{
    public string MaNcc { get; set; } = "";
    public string TenNcc { get; set; } = "";
    public string? DiaChi { get; set; }
    public string? DienThoai { get; set; }
    public override string ToString() => $"{MaNcc} - {TenNcc}";
}

public class NhanVienItem
{
    public string MaNv { get; set; } = "";
    public string TenNv { get; set; } = "";
    public string? DienThoai { get; set; }
    public override string ToString() => $"{MaNv} - {TenNv}";
}

public class ReportSpBanChayItem
{
    public string MaSp { get; set; } = "";
    public string TenSp { get; set; } = "";
    public string TenLoai { get; set; } = "";
    public string TenNhanHieu { get; set; } = "";
    public int TongSoLuong { get; set; }
    public decimal TongDoanhThu { get; set; }
}

public class ReportHdnLonNhatItem
{
    public string MaHdn { get; set; } = "";
    public DateTime NgayNhap { get; set; }
    public string TenNcc { get; set; } = "";
    public string TenNv { get; set; } = "";
    public decimal TongTien { get; set; }
}

public class ReportDoanhSoNamItem
{
    public string MaHdb { get; set; } = "";
    public DateTime NgayBan { get; set; }
    public string TenKhach { get; set; } = "";
    public string TenNv { get; set; } = "";
    public decimal TongTien { get; set; }
}

public class ReportNvDoanhSoThapItem
{
    public string MaNv { get; set; } = "";
    public string TenNv { get; set; } = "";
    public string GioiTinh { get; set; } = "";
    public string DienThoai { get; set; } = "";
    public decimal TongTienBan { get; set; }
}

public class ReportTonKhoItem
{
    public int Stt { get; set; }
    public string MaSp { get; set; } = "";
    public string TenSp { get; set; } = "";
    public string TenNhanHieu { get; set; } = "";
    public string TenLoai { get; set; } = "";
    public int NhapSl { get; set; }
    public decimal NhapTien { get; set; }
    public int XuatSl { get; set; }
    public decimal XuatTien { get; set; }
    public int TonCuoiSl { get; set; }
    public decimal TonCuoiTien { get; set; }
    public string TrangThai { get; set; } = "Còn hàng";
}

public class InvoiceSearchItem
{
    public string MaHdb { get; set; } = "";
    public DateTime NgayBan { get; set; }
    public decimal TongTien { get; set; }
    public string TenKhach { get; set; } = "";
    public string DienThoai { get; set; } = "";
    public string TenNv { get; set; } = "";
}

public class InboundInvoiceSearchItem
{
    public string MaHdn { get; set; } = "";
    public DateTime NgayNhap { get; set; }
    public decimal TongTien { get; set; }
    public string MaNcc { get; set; } = "";
    public string TenNcc { get; set; } = "";
    public string MaNv { get; set; } = "";
    public string TenNv { get; set; } = "";
    public int SoMatHang { get; set; }
}

public class InvoiceDetailItem
{
    public string MaSp { get; set; } = "";
    public string TenSp { get; set; } = "";
    public int SoLuong { get; set; }
    public decimal DonGia { get; set; }
    public decimal KhuyenMai { get; set; }
    public decimal ThanhTien { get; set; }
}

