using NexusRetail.Shared.DTOs.BanHang;
using NexusRetail.Shared.DTOs.SanPham;

namespace NexusRetail.WinForms.Services
{
    public class LoaiFilterItem
    {
        public string MaLoai { get; set; } = "";
        public string TenLoai { get; set; } = "";
    }

    public class NhanHieuFilterItem
    {
        public string MaNhanHieu { get; set; } = "";
        public string TenNhanHieu { get; set; } = "";
    }

    public class ManHinhFilterItem
    {
        public string MaManHinh { get; set; } = "";
        public string TenManHinh { get; set; } = "";
    }

    public class CategoryFilterResult
    {
        public List<LoaiFilterItem> Loai { get; set; } = new();
        public List<NhanHieuFilterItem> NhanHieu { get; set; } = new();
        public List<ManHinhFilterItem> ManHinh { get; set; } = new();
    }

    /// <summary>
    /// Hợp đồng dịch vụ xác thực người dùng cho ứng dụng WinForms POS.
    /// </summary>
    public interface IPosAuthClient
    {
        string? CurrentUser { get; }
        string? CurrentRole { get; }
        Task<(bool Success, string Role, string Message)> LoginAsync(string username, string password);
        void Logout();
    }

    /// <summary>
    /// Hợp đồng dịch vụ quản lý danh mục sản phẩm và bộ lọc thuộc tính.
    /// </summary>
    public interface IPosCatalogClient
    {
        Task<List<SanPhamDto>> GetProductsAsync(string? q = null);
        Task<CategoryFilterResult?> GetCategoryFilterAsync();
        Task<(bool Success, string Message, string? MaSp)> CreateProductAsync(ThemSanPhamDto dto);
        Task<(bool Success, string Message)> UpdateProductAsync(string maSp, SuaSanPhamDto dto);
        Task<(bool Success, string Message)> DeleteProductAsync(string maSp);
    }

    /// <summary>
    /// Hợp đồng dịch vụ bán hàng tại quầy, tra cứu hóa đơn và kiểm tra thanh toán QR.
    /// </summary>
    public interface IPosSalesClient
    {
        Task<string?> GetNextInvoiceCodeAsync();
        Task<(bool Success, string Message, string? MaHdb, decimal TongTien)> CreateSaleOrderAsync(HoaDonBanDto dto);
        Task<PaymentCheckResult> CheckBankPaymentStatusAsync(string maHdb, decimal? expectedAmount = null);
        Task<(bool Success, string Message)> SimulateBankPaymentAsync(string maHdb, decimal amount);
        Task<List<InvoiceSearchItem>> SearchSaleInvoicesAsync(string? q = null);
        Task<List<InvoiceDetailItem>> GetSaleInvoiceDetailsAsync(string maHdb);
    }

    /// <summary>
    /// Hợp đồng dịch vụ nhập hàng từ nhà cung cấp và quản lý phiếu nhập kho.
    /// </summary>
    public interface IPosInboundClient
    {
        Task<List<NhaCungCapItem>> GetSuppliersAsync();
        Task<List<NhanVienItem>> GetEmployeesAsync();
        Task<string?> GetNextInboundInvoiceCodeAsync();
        Task<(bool Success, string Message, string? MaHdn, decimal TongTien)> CreateInboundOrderAsync(NexusRetail.Shared.DTOs.NhapHang.HoaDonNhapDto dto);
        Task<List<InboundInvoiceSearchItem>> SearchInboundInvoicesAsync();
        Task<List<InvoiceDetailItem>> GetInboundInvoiceDetailsAsync(string maHdn);
    }

    /// <summary>
    /// Hợp đồng dịch vụ báo cáo thống kê doanh thu, tồn kho và xuất tệp Excel.
    /// </summary>
    public interface IPosReportClient
    {
        Task<List<ReportSpBanChayItem>> GetReportSpBanChayAsync(int month, int year);
        Task<List<ReportHdnLonNhatItem>> GetReportHdnLonNhatAsync(int quarter, int year);
        Task<List<ReportDoanhSoNamItem>> GetReportDoanhSoNamAsync(int year);
        Task<List<ReportNvDoanhSoThapItem>> GetReportNvDoanhSoThapAsync(int month, int year);
        Task<List<ReportTonKhoItem>> GetReportTonKhoAsync(int month, int year);
        Task<(bool Success, string Message)> DownloadExcelAsync(string relativeUrl, string saveFilePath);
    }
}

