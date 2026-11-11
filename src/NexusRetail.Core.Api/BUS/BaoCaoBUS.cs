using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NexusRetail.Core.Api.Data;
using NexusRetail.Core.Api.Services.Export;

namespace NexusRetail.Core.Api.BUS;

public record LowStockProductDto(
    string MaSp,
    string TenSp,
    string TenNhanHieu,
    string TenLoai,
    int SoLuong,
    decimal GiaNhap,
    decimal GiaBan);

public record TopSellingProductRow(string MaSp, string TenSp, string TenLoai, string TenNhanHieu, int TongSoLuong, int TongSoLuongBan, decimal TongDoanhThu);
public record LargestInboundRow(string MaHdn, DateTime NgayNhap, string MaNcc, string TenNcc, string MaNv, string TenNv, decimal TongTien);
public record AnnualRevenueRow(string MaHdb, DateTime NgayBan, string MaKhach, string TenKhach, string MaNv, string TenNv, decimal TongTien);
public record LowPerformingStaffRow(string MaNv, string TenNv, string GioiTinh, string DienThoai, int SoHoaDon, decimal TongTienBan);
public record InventorySummaryRow(int Stt, string MaSp, string TenSp, string TenNhanHieu, string TenLoai, int NhapSl, decimal NhapTien, int XuatSl, decimal XuatTien, int TonCuoiSl, decimal TonCuoiTien, string TrangThai);

public interface IBaoCaoBUS
{
    Task<object> LaySpBanChayAsync(int month, int year);
    Task<byte[]> XuatExcelSpBanChayAsync(int month, int year);
    Task<object> LayHdnLonNhatAsync(int quarter, int year);
    Task<byte[]> XuatExcelHdnLonNhatAsync(int quarter, int year);
    Task<object> LayDoanhSoNamAsync(int year);
    Task<byte[]> XuatExcelDoanhSoNamAsync(int year);
    Task<object> LayNvDoanhSoThapAsync(int month, int year);
    Task<byte[]> XuatExcelNvDoanhSoThapAsync(int month, int year);
    Task<object> LayBaoCaoTonKhoAsync(int month, int year);
    Task<byte[]> XuatExcelBaoCaoTonKhoAsync(int month, int year);
    Task<List<LowStockProductDto>> LayCanhBaoTonKhoAdoNetAsync(int nguong = 5);
}

/// <summary>
/// Tầng nghiệp vụ (BUS) báo cáo thống kê: ưu tiên gọi Stored Procedure và SQL Text qua ADO.NET
/// khi chạy với SQL Server thực tế, tự động fallback sang LINQ cho kiểm thử InMemory.
/// </summary>
public class BaoCaoBUS : IBaoCaoBUS
{
    private readonly RetailDbContext _db;
    private readonly IExcelExportService _excelService;
    private readonly IAdoNetDataProvider? _adoNet;
    private readonly ILogger<BaoCaoBUS>? _logger;

    public BaoCaoBUS(
        RetailDbContext db,
        IExcelExportService excelService,
        IAdoNetDataProvider? adoNet = null,
        ILogger<BaoCaoBUS>? logger = null)
    {
        _db = db;
        _excelService = excelService;
        _adoNet = adoNet;
        _logger = logger;
    }

    private bool UseAdoNet => _adoNet != null && _db.Database.IsSqlServer();

    private static string ReadString(DataRow row, string col, string fallback = "")
        => row.Table.Columns.Contains(col) && row[col] != DBNull.Value ? row[col].ToString() ?? fallback : fallback;

    private static int ReadInt(DataRow row, string col)
        => row.Table.Columns.Contains(col) && row[col] != DBNull.Value ? Convert.ToInt32(row[col]) : 0;

    private static decimal ReadDecimal(DataRow row, string col)
        => row.Table.Columns.Contains(col) && row[col] != DBNull.Value ? Convert.ToDecimal(row[col]) : 0m;

    private static DateTime ReadDateTime(DataRow row, string col)
        => row.Table.Columns.Contains(col) && row[col] != DBNull.Value ? Convert.ToDateTime(row[col]) : DateTime.Now;

    #region 1. Top 5 Sản phẩm bán chạy (Stored Procedure: sp_BaoCao_SP_BanChayNhat_Thang)
    private async Task<List<TopSellingProductRow>> GetSpBanChayDataAsync(int m, int y)
    {
        if (UseAdoNet)
        {
            try
            {
                DataTable dt = _adoNet!.SpBaoCaoSanPhamBanChay(m, y);
                return dt.AsEnumerable()
                    .Select(r =>
                    {
                        int sl = ReadInt(r, "TongSoLuongBan");
                        return new TopSellingProductRow(
                            ReadString(r, "MaSP"),
                            ReadString(r, "TenSP"),
                            ReadString(r, "TenLoai", "Điện thoại"),
                            ReadString(r, "TenNhanHieu", "Chính hãng"),
                            sl,
                            sl,
                            ReadDecimal(r, "TongDoanhThu"));
                    })
                    .Take(5)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "[BaoCaoBUS] Fallback LINQ cho sp_BaoCao_SP_BanChayNhat_Thang.");
            }
        }

        var raw = await _db.ChiTietHdbs
            .AsNoTracking()
            .Where(x => x.MaHdbNavigation.NgayBan.Month == m && x.MaHdbNavigation.NgayBan.Year == y)
            .Select(x => new
            {
                x.MaSp,
                TenSp = x.MaSpNavigation.TenSp,
                TenLoai = x.MaSpNavigation.MaLoaiNavigation != null ? x.MaSpNavigation.MaLoaiNavigation.TenLoai : "Điện thoại",
                TenNhanHieu = x.MaSpNavigation.MaNhanHieuNavigation != null ? x.MaSpNavigation.MaNhanHieuNavigation.TenNhanHieu : "Chính hãng",
                x.SoLuong,
                x.ThanhTien
            })
            .ToListAsync();

        return raw
            .GroupBy(x => new { x.MaSp, x.TenSp, x.TenLoai, x.TenNhanHieu })
            .Select(g => new TopSellingProductRow(
                g.Key.MaSp,
                g.Key.TenSp,
                g.Key.TenLoai,
                g.Key.TenNhanHieu,
                g.Sum(x => x.SoLuong),
                g.Sum(x => x.SoLuong),
                g.Sum(x => x.ThanhTien)))
            .OrderByDescending(x => x.TongSoLuong)
            .Take(5)
            .ToList();
    }

    public async Task<object> LaySpBanChayAsync(int month, int year)
    {
        var list = await GetSpBanChayDataAsync(month, year);
        return new
        {
            Thang = month,
            Nam = year,
            TongSoMatHang = list.Count,
            TongSoLuongBan = list.Sum(x => x.TongSoLuong),
            TongDoanhThu = list.Sum(x => x.TongDoanhThu),
            TopSanPham = list,
            DanhSach = list
        };
    }

    public async Task<byte[]> XuatExcelSpBanChayAsync(int month, int year)
    {
        var data = await GetSpBanChayDataAsync(month, year);
        var headers = new[] { "STT", "Mã Sản Phẩm", "Tên Sản Phẩm", "Phân Loại", "Nhãn Hiệu", "Số Lượng Bán", "Tổng Doanh Thu" };
        var rows = data.Select((item, i) => new object[]
        {
            i + 1, item.MaSp, item.TenSp, item.TenLoai, item.TenNhanHieu, item.TongSoLuong, item.TongDoanhThu
        }).ToList();

        return _excelService.GenerateExcelReport($"BÁO CÁO TOP 5 SẢN PHẨM BÁN CHẠY NHẤT THÁNG {month}/{year}", headers, rows);
    }
    #endregion

    #region 2. Hóa đơn nhập lớn nhất quý (Stored Procedure: sp_BaoCao_HDNhap_LonNhat_Quy)
    private async Task<List<LargestInboundRow>> GetHdnLonNhatDataAsync(int q, int y)
    {
        if (UseAdoNet)
        {
            try
            {
                DataTable dt = _adoNet!.SpBaoCaoHdnLonNhatQuy(q, y);
                return dt.AsEnumerable()
                    .Select(r => new LargestInboundRow(
                        ReadString(r, "MaHDN"),
                        ReadDateTime(r, "NgayNhap"),
                        ReadString(r, "MaNCC"),
                        ReadString(r, "NhaCungCap"),
                        ReadString(r, "MaNV"),
                        ReadString(r, "NguoiNhap"),
                        ReadDecimal(r, "TongTien")))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "[BaoCaoBUS] Fallback LINQ cho sp_BaoCao_HDNhap_LonNhat_Quy.");
            }
        }

        int startMonth = (q - 1) * 3 + 1;
        int endMonth = startMonth + 2;

        return await _db.HoaDonNhaps
            .AsNoTracking()
            .Include(x => x.MaNccNavigation)
            .Include(x => x.MaNvNavigation)
            .Where(x => x.NgayNhap.Year == y && x.NgayNhap.Month >= startMonth && x.NgayNhap.Month <= endMonth)
            .OrderByDescending(x => x.TongTien)
            .Select(x => new LargestInboundRow(
                x.MaHdn,
                x.NgayNhap,
                x.MaNcc,
                x.MaNccNavigation != null ? x.MaNccNavigation.TenNcc : x.MaNcc,
                x.MaNv,
                x.MaNvNavigation != null ? x.MaNvNavigation.TenNv : x.MaNv,
                x.TongTien))
            .ToListAsync();
    }

    public async Task<object> LayHdnLonNhatAsync(int quarter, int year)
    {
        var list = await GetHdnLonNhatDataAsync(quarter, year);
        return new
        {
            Quy = quarter,
            Nam = year,
            TongTienNhapQuy = list.Sum(x => x.TongTien),
            TongSoHoaDon = list.Count,
            HoaDonLonNhat = list.FirstOrDefault(),
            DanhSachHoaDon = list,
            DanhSach = list
        };
    }

    public async Task<byte[]> XuatExcelHdnLonNhatAsync(int quarter, int year)
    {
        var list = await GetHdnLonNhatDataAsync(quarter, year);
        var headers = new[] { "STT", "Mã HĐ Nhập", "Ngày Nhập", "Nhà Cung Cấp", "Nhân Viên Nhập", "Tổng Tiền Nhập" };
        var rows = list.Select((item, i) => new object[]
        {
            i + 1, item.MaHdn, item.NgayNhap.ToString("dd/MM/yyyy"), item.TenNcc, item.TenNv, item.TongTien
        }).ToList();

        return _excelService.GenerateExcelReport($"BÁO CÁO HÓA ĐƠN NHẬP LỚN NHẤT QUÝ {quarter} NĂM {year}", headers, rows);
    }
    #endregion

    #region 3. Doanh số bán theo năm (Stored Procedure: sp_BaoCao_DoanhSo_Nam)
    private async Task<List<AnnualRevenueRow>> GetDoanhSoNamDataAsync(int y)
    {
        if (UseAdoNet)
        {
            try
            {
                DataTable dt = _adoNet!.SpBaoCaoDoanhSoNam(y);
                return dt.AsEnumerable()
                    .Select(r => new AnnualRevenueRow(
                        ReadString(r, "MaHDB"),
                        ReadDateTime(r, "NgayBan"),
                        ReadString(r, "MaKhach"),
                        ReadString(r, "TenKhach"),
                        ReadString(r, "MaNV"),
                        ReadString(r, "NhanVienBan"),
                        ReadDecimal(r, "TongTien")))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "[BaoCaoBUS] Fallback LINQ cho sp_BaoCao_DoanhSo_Nam.");
            }
        }

        return await _db.HoaDonBans
            .AsNoTracking()
            .Include(x => x.MaKhachNavigation)
            .Include(x => x.MaNvNavigation)
            .Where(x => x.NgayBan.Year == y)
            .OrderBy(x => x.NgayBan)
            .Select(x => new AnnualRevenueRow(
                x.MaHdb,
                x.NgayBan,
                x.MaKhach,
                x.MaKhachNavigation != null ? x.MaKhachNavigation.TenKhach : x.MaKhach,
                x.MaNv,
                x.MaNvNavigation != null ? x.MaNvNavigation.TenNv : x.MaNv,
                x.TongTien))
            .ToListAsync();
    }

    public async Task<object> LayDoanhSoNamAsync(int year)
    {
        var list = await GetDoanhSoNamDataAsync(year);
        return new
        {
            Nam = year,
            TongDoanhThu = list.Sum(x => x.TongTien),
            TongSoDonHang = list.Count,
            DanhSachHoaDon = list,
            DanhSach = list
        };
    }

    public async Task<byte[]> XuatExcelDoanhSoNamAsync(int year)
    {
        var list = await GetDoanhSoNamDataAsync(year);
        var headers = new[] { "STT", "Mã HĐ Bán", "Ngày Bán", "Khách Hàng", "Nhân Viên Bán", "Tổng Tiền" };
        var rows = list.Select((item, i) => new object[]
        {
            i + 1, item.MaHdb, item.NgayBan.ToString("dd/MM/yyyy HH:mm"), item.TenKhach, item.TenNv, item.TongTien
        }).ToList();

        return _excelService.GenerateExcelReport($"BÁO CÁO TỔNG KẾT DOANH SỐ BÁN NĂM {year}", headers, rows);
    }
    #endregion

    #region 4. 5 Nhân viên doanh số thấp nhất tháng (Stored Procedure: sp_BaoCao_5NV_BanItNhat_Thang)
    private async Task<List<LowPerformingStaffRow>> GetNvDoanhSoThapDataAsync(int m, int y)
    {
        if (UseAdoNet)
        {
            try
            {
                DataTable dt = _adoNet!.SpBaoCao5NvBanItNhat(m, y);
                var genderLookup = await _db.NhanViens
                    .AsNoTracking()
                    .ToDictionaryAsync(x => x.MaNv, x => x.GioiTinh ?? "Nam");

                return dt.AsEnumerable()
                    .Select(r =>
                    {
                        string maNv = ReadString(r, "MaNV");
                        string gioiTinh = genderLookup.TryGetValue(maNv, out var gt) ? gt : "Nam";
                        return new LowPerformingStaffRow(
                            maNv,
                            ReadString(r, "TenNV"),
                            gioiTinh,
                            ReadString(r, "DienThoai"),
                            ReadInt(r, "SoHoaDonLap"),
                            ReadDecimal(r, "TongTienBanDuoc"));
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "[BaoCaoBUS] Fallback LINQ cho sp_BaoCao_5NV_BanItNhat_Thang.");
            }
        }

        return await _db.NhanViens
            .AsNoTracking()
            .Select(nv => new LowPerformingStaffRow(
                nv.MaNv,
                nv.TenNv,
                nv.GioiTinh ?? "Nam",
                nv.DienThoai ?? "",
                _db.HoaDonBans.Count(h => h.MaNv == nv.MaNv && h.NgayBan.Month == m && h.NgayBan.Year == y),
                _db.HoaDonBans
                    .Where(h => h.MaNv == nv.MaNv && h.NgayBan.Month == m && h.NgayBan.Year == y)
                    .Sum(h => (decimal?)h.TongTien) ?? 0m))
            .OrderBy(x => x.TongTienBan)
            .Take(5)
            .ToListAsync();
    }

    public async Task<object> LayNvDoanhSoThapAsync(int month, int year)
    {
        var list = await GetNvDoanhSoThapDataAsync(month, year);
        return new
        {
            Thang = month,
            Nam = year,
            DanhSachNhanVien = list,
            DanhSach = list
        };
    }

    public async Task<byte[]> XuatExcelNvDoanhSoThapAsync(int month, int year)
    {
        var list = await GetNvDoanhSoThapDataAsync(month, year);
        var headers = new[] { "STT", "Mã Nhân Viên", "Tên Nhân Viên", "Giới Tính", "Số Điện Thoại", "Doanh Số Bán Trong Tháng" };
        var rows = list.Select((item, i) => new object[]
        {
            i + 1, item.MaNv, item.TenNv, item.GioiTinh, item.DienThoai, item.TongTienBan
        }).ToList();

        return _excelService.GenerateExcelReport($"BÁO CÁO 5 NHÂN VIÊN DOANH SỐ THẤP NHẤT THÁNG {month}/{year}", headers, rows);
    }
    #endregion

    #region 5. Xuất - Nhập - Tồn kho & Cảnh báo tồn kho ADO.NET (SQL Text)
    private async Task<List<InventorySummaryRow>> GetTonKhoDataAsync(int m, int y)
    {
        var products = await _db.SanPhams
            .AsNoTracking()
            .Include(x => x.MaNhanHieuNavigation)
            .Include(x => x.MaLoaiNavigation)
            .OrderBy(x => x.MaSp)
            .ToListAsync();

        var nhapDict = await _db.ChiTietHdns
            .AsNoTracking()
            .Where(x => x.MaHdnNavigation.NgayNhap.Month == m && x.MaHdnNavigation.NgayNhap.Year == y)
            .GroupBy(x => x.MaSp)
            .Select(g => new { MaSp = g.Key, SoLuong = g.Sum(x => x.SoLuong), TongTien = g.Sum(x => x.ThanhTien) })
            .ToDictionaryAsync(x => x.MaSp, x => x);

        var xuatDict = await _db.ChiTietHdbs
            .AsNoTracking()
            .Where(x => x.MaHdbNavigation.NgayBan.Month == m && x.MaHdbNavigation.NgayBan.Year == y)
            .GroupBy(x => x.MaSp)
            .Select(g => new { MaSp = g.Key, SoLuong = g.Sum(x => x.SoLuong), TongTien = g.Sum(x => x.ThanhTien) })
            .ToDictionaryAsync(x => x.MaSp, x => x);

        return products.Select((p, idx) =>
        {
            int tonCuoi = p.SoLuong;
            int nhap = nhapDict.TryGetValue(p.MaSp, out var nVal) ? nVal.SoLuong : 0;
            decimal nhapTien = nVal != null ? nVal.TongTien : 0m;
            int xuat = xuatDict.TryGetValue(p.MaSp, out var xVal) ? xVal.SoLuong : 0;
            decimal xuatTien = xVal != null ? xVal.TongTien : 0m;
            decimal giaTriTon = tonCuoi * p.GiaNhap;
            string trangThai = tonCuoi > 15 ? "Tồn nhiều" : (tonCuoi > 0 ? "Còn hàng" : "Hết hàng");

            return new InventorySummaryRow(
                idx + 1,
                p.MaSp,
                p.TenSp,
                p.MaNhanHieuNavigation?.TenNhanHieu ?? "Chính hãng",
                p.MaLoaiNavigation?.TenLoai ?? "Điện thoại",
                nhap,
                nhapTien,
                xuat,
                xuatTien,
                tonCuoi,
                giaTriTon,
                trangThai);
        }).ToList();
    }

    public async Task<object> LayBaoCaoTonKhoAsync(int month, int year)
    {
        var list = await GetTonKhoDataAsync(month, year);
        return new
        {
            Thang = month,
            Nam = year,
            TongSoThietBi = list.Count,
            TongTonKho = list.Sum(x => x.TonCuoiSl),
            TongGiaTriTonKho = list.Sum(x => x.TonCuoiTien),
            DanhSach = list
        };
    }

    public async Task<byte[]> XuatExcelBaoCaoTonKhoAsync(int month, int year)
    {
        var list = await GetTonKhoDataAsync(month, year);
        var headers = new[] { "STT", "Mã Sản Phẩm", "Tên Thiết Bị", "Thương Hiệu", "Nhập Trong Kỳ", "Tiền Nhập (₫)", "Xuất Trong Kỳ", "Doanh Thu Xuất (₫)", "Tồn Cuối Kỳ", "Giá Trị Tồn (₫)", "Trạng Thái" };
        var rows = list.Select(item => new object[]
        {
            item.Stt, item.MaSp, item.TenSp, item.TenNhanHieu, item.NhapSl, item.NhapTien, item.XuatSl, item.XuatTien, item.TonCuoiSl, item.TonCuoiTien, item.TrangThai
        }).ToList();

        return _excelService.GenerateExcelReport($"BÁO CÁO XUẤT NHẬP TỒN KHO THÁNG {month}/{year}", headers, rows);
    }

    public async Task<List<LowStockProductDto>> LayCanhBaoTonKhoAdoNetAsync(int nguong = 5)
    {
        if (UseAdoNet)
        {
            const string sql = @"
                SELECT sp.MaSP, sp.TenSP,
                       ISNULL(nh.TenNhanHieu, N'Chính hãng') AS TenNhanHieu,
                       ISNULL(l.TenLoai, N'Điện thoại') AS TenLoai,
                       sp.SoLuong, sp.GiaNhap, sp.GiaBan
                FROM SanPham sp
                LEFT JOIN NhanHieu nh ON sp.MaNhanHieu = nh.MaNhanHieu
                LEFT JOIN Loai l ON sp.MaLoai = l.MaLoai
                WHERE sp.SoLuong <= @Nguong
                ORDER BY sp.SoLuong ASC, sp.MaSP ASC";

            var parameters = new[] { new SqlParameter("@Nguong", SqlDbType.Int) { Value = nguong } };
            DataTable dt = _adoNet!.ExecuteQueryText(sql, parameters);

            return dt.AsEnumerable()
                .Select(r => new LowStockProductDto(
                    ReadString(r, "MaSP"),
                    ReadString(r, "TenSP"),
                    ReadString(r, "TenNhanHieu", "Chính hãng"),
                    ReadString(r, "TenLoai", "Điện thoại"),
                    ReadInt(r, "SoLuong"),
                    ReadDecimal(r, "GiaNhap"),
                    ReadDecimal(r, "GiaBan")))
                .ToList();
        }

        return await _db.SanPhams
            .AsNoTracking()
            .Include(x => x.MaNhanHieuNavigation)
            .Include(x => x.MaLoaiNavigation)
            .Where(x => x.SoLuong <= nguong)
            .OrderBy(x => x.SoLuong)
            .ThenBy(x => x.MaSp)
            .Select(x => new LowStockProductDto(
                x.MaSp,
                x.TenSp,
                x.MaNhanHieuNavigation != null ? x.MaNhanHieuNavigation.TenNhanHieu : "Chính hãng",
                x.MaLoaiNavigation != null ? x.MaLoaiNavigation.TenLoai : "Điện thoại",
                x.SoLuong,
                x.GiaNhap,
                x.GiaBan))
            .ToListAsync();
    }
    #endregion
}
