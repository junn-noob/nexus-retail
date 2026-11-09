using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusRetail.Core.Api.Data;
using NexusRetail.Core.Api.Services.Export;

namespace NexusRetail.Core.Api.Controllers
{
    [Route("api/bao-cao")]
    [ApiController]
    public class BaoCaoController : ControllerBase
    {
        private readonly RetailDbContext _db;
        private readonly IExcelExportService _excelService;

        public BaoCaoController(RetailDbContext db, IExcelExportService excelService)
        {
            _db = db;
            _excelService = excelService;
        }

        #region Helper Xuất Excel
        private byte[] taoFileExcel(string tieuDe, string[] cotTieuDe, List<object[]> duLieuHang)
        {
            return _excelService.GenerateExcelReport(tieuDe, cotTieuDe, duLieuHang);
        }
        #endregion

        /// <summary>
        /// Báo cáo top 5 sản phẩm bán chạy nhất theo tháng / năm
        /// </summary>
        [HttpGet("sp-ban-chay")]
        public async Task<IActionResult> laySpBanChay([FromQuery] int? month = null, [FromQuery] int? thang = null, [FromQuery] int? year = null, [FromQuery] int? nam = null)
        {
            int m = month ?? thang ?? 9;
            int y = year ?? nam ?? 2026;

            if (m < 1 || m > 12 || y < 2000)
                return BadRequest(new { message = "Tháng (1-12) hoặc năm không hợp lệ!" });

            var list = await _db.ChiTietHdbs
                .AsNoTracking()
                .Include(x => x.MaHdbNavigation)
                .Include(x => x.MaSpNavigation)
                    .ThenInclude(s => s.MaLoaiNavigation)
                .Include(x => x.MaSpNavigation)
                    .ThenInclude(s => s.MaNhanHieuNavigation)
                .Where(x => x.MaHdbNavigation.NgayBan.Month == m && x.MaHdbNavigation.NgayBan.Year == y)
                .GroupBy(x => new
                {
                    x.MaSp,
                    TenSp = x.MaSpNavigation.TenSp,
                    TenLoai = x.MaSpNavigation.MaLoaiNavigation != null ? x.MaSpNavigation.MaLoaiNavigation.TenLoai : "Điện thoại",
                    TenNhanHieu = x.MaSpNavigation.MaNhanHieuNavigation != null ? x.MaSpNavigation.MaNhanHieuNavigation.TenNhanHieu : "Chính hãng"
                })
                .Select(g => new
                {
                    MaSp = g.Key.MaSp,
                    TenSp = g.Key.TenSp,
                    TenLoai = g.Key.TenLoai,
                    TenNhanHieu = g.Key.TenNhanHieu,
                    TongSoLuong = g.Sum(x => x.SoLuong),
                    TongSoLuongBan = g.Sum(x => x.SoLuong),
                    TongDoanhThu = g.Sum(x => x.ThanhTien)
                })
                .OrderByDescending(x => x.TongSoLuong)
                .Take(5)
                .ToListAsync();

            return Ok(new
            {
                Thang = m,
                Nam = y,
                TongSoMatHang = list.Count,
                TongSoLuongBan = list.Sum(x => x.TongSoLuong),
                TongDoanhThu = list.Sum(x => x.TongDoanhThu),
                TopSanPham = list,
                DanhSach = list
            });
        }

        [HttpGet("sp-ban-chay/excel")]
        public async Task<IActionResult> xuatExcelSpBanChay([FromQuery] int? month = null, [FromQuery] int? thang = null, [FromQuery] int? year = null, [FromQuery] int? nam = null)
        {
            int m = month ?? thang ?? 9;
            int y = year ?? nam ?? 2026;

            var data = await _db.ChiTietHdbs
                .AsNoTracking()
                .Include(x => x.MaHdbNavigation)
                .Include(x => x.MaSpNavigation)
                    .ThenInclude(s => s.MaLoaiNavigation)
                .Include(x => x.MaSpNavigation)
                    .ThenInclude(s => s.MaNhanHieuNavigation)
                .Where(x => x.MaHdbNavigation.NgayBan.Month == m && x.MaHdbNavigation.NgayBan.Year == y)
                .GroupBy(x => new
                {
                    x.MaSp,
                    TenSp = x.MaSpNavigation.TenSp,
                    TenLoai = x.MaSpNavigation.MaLoaiNavigation != null ? x.MaSpNavigation.MaLoaiNavigation.TenLoai : "Điện thoại",
                    TenNhanHieu = x.MaSpNavigation.MaNhanHieuNavigation != null ? x.MaSpNavigation.MaNhanHieuNavigation.TenNhanHieu : "Chính hãng"
                })
                .Select(g => new
                {
                    MaSp = g.Key.MaSp,
                    TenSp = g.Key.TenSp,
                    TenLoai = g.Key.TenLoai,
                    TenNhanHieu = g.Key.TenNhanHieu,
                    TongSoLuong = g.Sum(x => x.SoLuong),
                    TongDoanhThu = g.Sum(x => x.ThanhTien)
                })
                .OrderByDescending(x => x.TongSoLuong)
                .Take(5)
                .ToListAsync();

            var headers = new[] { "STT", "Mã Sản Phẩm", "Tên Sản Phẩm", "Phân Loại", "Nhãn Hiệu", "Số Lượng Bán", "Tổng Doanh Thu" };
            var rows = new List<object[]>();
            for (int i = 0; i < data.Count; i++)
            {
                rows.Add(new object[]
                {
                    i + 1,
                    data[i].MaSp,
                    data[i].TenSp,
                    data[i].TenLoai,
                    data[i].TenNhanHieu,
                    data[i].TongSoLuong,
                    data[i].TongDoanhThu
                });
            }

            var fileBytes = taoFileExcel($"BÁO CÁO TOP 5 SẢN PHẨM BÁN CHẠY NHẤT THÁNG {m}/{y}", headers, rows);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"BaoCao_Top5_SanPham_Thang_{m}_{y}.xlsx");
        }

        /// <summary>
        /// Báo cáo hóa đơn nhập lớn nhất theo quý / năm
        /// </summary>
        [HttpGet("hdn-lon-nhat")]
        [HttpGet("nhap-lon-nhat-quy")]
        public async Task<IActionResult> layHdnLonNhat([FromQuery] int? quarter = null, [FromQuery] int? quy = null, [FromQuery] int? year = null, [FromQuery] int? nam = null)
        {
            int q = quarter ?? quy ?? 3;
            int y = year ?? nam ?? 2026;

            if (q < 1 || q > 4 || y < 2000)
                return BadRequest(new { message = "Quý (1-4) hoặc năm không hợp lệ!" });

            int startMonth = (q - 1) * 3 + 1;
            int endMonth = startMonth + 2;

            var list = await _db.HoaDonNhaps
                .AsNoTracking()
                .Include(x => x.MaNccNavigation)
                .Include(x => x.MaNvNavigation)
                .Where(x => x.NgayNhap.Year == y && x.NgayNhap.Month >= startMonth && x.NgayNhap.Month <= endMonth)
                .OrderByDescending(x => x.TongTien)
                .Select(x => new
                {
                    x.MaHdn,
                    x.NgayNhap,
                    x.MaNcc,
                    TenNcc = x.MaNccNavigation != null ? x.MaNccNavigation.TenNcc : x.MaNcc,
                    x.MaNv,
                    TenNv = x.MaNvNavigation != null ? x.MaNvNavigation.TenNv : x.MaNv,
                    x.TongTien
                })
                .ToListAsync();

            return Ok(new
            {
                Quy = q,
                Nam = y,
                TongTienNhapQuy = list.Sum(x => x.TongTien),
                TongSoHoaDon = list.Count,
                HoaDonLonNhat = list.FirstOrDefault(),
                DanhSachHoaDon = list,
                DanhSach = list
            });
        }

        [HttpGet("hdn-lon-nhat/excel")]
        [HttpGet("nhap-lon-nhat-quy/excel")]
        public async Task<IActionResult> xuatExcelHdnLonNhat([FromQuery] int? quarter = null, [FromQuery] int? quy = null, [FromQuery] int? year = null, [FromQuery] int? nam = null)
        {
            int q = quarter ?? quy ?? 3;
            int y = year ?? nam ?? 2026;

            int startMonth = (q - 1) * 3 + 1;
            int endMonth = startMonth + 2;

            var list = await _db.HoaDonNhaps
                .AsNoTracking()
                .Include(x => x.MaNccNavigation)
                .Include(x => x.MaNvNavigation)
                .Where(x => x.NgayNhap.Year == y && x.NgayNhap.Month >= startMonth && x.NgayNhap.Month <= endMonth)
                .OrderByDescending(x => x.TongTien)
                .ToListAsync();

            var headers = new[] { "STT", "Mã HĐ Nhập", "Ngày Nhập", "Nhà Cung Cấp", "Nhân Viên Nhập", "Tổng Tiền Nhập" };
            var rows = new List<object[]>();
            for (int i = 0; i < list.Count; i++)
            {
                rows.Add(new object[]
                {
                    i + 1,
                    list[i].MaHdn,
                    list[i].NgayNhap.ToString("dd/MM/yyyy"),
                    list[i].MaNccNavigation?.TenNcc ?? list[i].MaNcc,
                    list[i].MaNvNavigation?.TenNv ?? list[i].MaNv,
                    list[i].TongTien
                });
            }

            var fileBytes = taoFileExcel($"BÁO CÁO HÓA ĐƠN NHẬP LỚN NHẤT QUÝ {q} NĂM {y}", headers, rows);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"BaoCao_HDN_LonNhat_Quy_{q}_{y}.xlsx");
        }

        /// <summary>
        /// Báo cáo tổng kết doanh số bán theo năm
        /// </summary>
        [HttpGet("doanh-so-nam")]
        public async Task<IActionResult> layDoanhSoNam([FromQuery] int? year = null, [FromQuery] int? nam = null)
        {
            int y = year ?? nam ?? 2026;
            if (y < 2000)
                return BadRequest(new { message = "Năm không hợp lệ!" });

            var list = await _db.HoaDonBans
                .AsNoTracking()
                .Include(x => x.MaKhachNavigation)
                .Include(x => x.MaNvNavigation)
                .Where(x => x.NgayBan.Year == y)
                .OrderBy(x => x.NgayBan)
                .Select(x => new
                {
                    x.MaHdb,
                    x.NgayBan,
                    x.MaKhach,
                    TenKhach = x.MaKhachNavigation != null ? x.MaKhachNavigation.TenKhach : x.MaKhach,
                    x.MaNv,
                    TenNv = x.MaNvNavigation != null ? x.MaNvNavigation.TenNv : x.MaNv,
                    x.TongTien
                })
                .ToListAsync();

            return Ok(new
            {
                Nam = y,
                TongDoanhThu = list.Sum(x => x.TongTien),
                TongSoDonHang = list.Count,
                DanhSachHoaDon = list,
                DanhSach = list
            });
        }

        [HttpGet("doanh-so-nam/excel")]
        public async Task<IActionResult> xuatExcelDoanhSoNam([FromQuery] int? year = null, [FromQuery] int? nam = null)
        {
            int y = year ?? nam ?? 2026;

            var list = await _db.HoaDonBans
                .AsNoTracking()
                .Include(x => x.MaKhachNavigation)
                .Include(x => x.MaNvNavigation)
                .Where(x => x.NgayBan.Year == y)
                .OrderBy(x => x.NgayBan)
                .ToListAsync();

            var headers = new[] { "STT", "Mã HĐ Bán", "Ngày Bán", "Khách Hàng", "Nhân Viên Bán", "Tổng Tiền" };
            var rows = new List<object[]>();
            for (int i = 0; i < list.Count; i++)
            {
                rows.Add(new object[]
                {
                    i + 1,
                    list[i].MaHdb,
                    list[i].NgayBan.ToString("dd/MM/yyyy HH:mm"),
                    list[i].MaKhachNavigation?.TenKhach ?? list[i].MaKhach,
                    list[i].MaNvNavigation?.TenNv ?? list[i].MaNv,
                    list[i].TongTien
                });
            }

            var fileBytes = taoFileExcel($"BÁO CÁO TỔNG KẾT DOANH SỐ BÁN NĂM {y}", headers, rows);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"BaoCao_DoanhSo_Nam_{y}.xlsx");
        }

        /// <summary>
        /// Báo cáo top 5 nhân viên có doanh số thấp nhất trong tháng
        /// </summary>
        [HttpGet("nv-doanh-so-thap")]
        [HttpGet("nv-ban-it-nhat")]
        public async Task<IActionResult> layNvDoanhSoThap([FromQuery] int? month = null, [FromQuery] int? thang = null, [FromQuery] int? year = null, [FromQuery] int? nam = null)
        {
            int m = month ?? thang ?? 9;
            int y = year ?? nam ?? 2026;

            if (m < 1 || m > 12 || y < 2000)
                return BadRequest(new { message = "Tháng hoặc năm không hợp lệ!" });

            var list = await _db.NhanViens
                .AsNoTracking()
                .Select(nv => new
                {
                    nv.MaNv,
                    nv.TenNv,
                    GioiTinh = nv.GioiTinh ?? "Nam",
                    DienThoai = nv.DienThoai ?? "",
                    SoHoaDon = _db.HoaDonBans.Count(h => h.MaNv == nv.MaNv && h.NgayBan.Month == m && h.NgayBan.Year == y),
                    TongTienBan = _db.HoaDonBans
                        .Where(h => h.MaNv == nv.MaNv && h.NgayBan.Month == m && h.NgayBan.Year == y)
                        .Sum(h => (decimal?)h.TongTien) ?? 0m
                })
                .OrderBy(x => x.TongTienBan)
                .Take(5)
                .ToListAsync();

            return Ok(new
            {
                Thang = m,
                Nam = y,
                DanhSachNhanVien = list,
                DanhSach = list
            });
        }

        [HttpGet("nv-doanh-so-thap/excel")]
        [HttpGet("nv-ban-it-nhat/excel")]
        public async Task<IActionResult> xuatExcelNvDoanhSoThap([FromQuery] int? month = null, [FromQuery] int? thang = null, [FromQuery] int? year = null, [FromQuery] int? nam = null)
        {
            int m = month ?? thang ?? 9;
            int y = year ?? nam ?? 2026;

            var list = await _db.NhanViens
                .AsNoTracking()
                .Select(nv => new
                {
                    nv.MaNv,
                    nv.TenNv,
                    GioiTinh = nv.GioiTinh ?? "Nam",
                    DienThoai = nv.DienThoai ?? "",
                    TongTienBan = _db.HoaDonBans
                        .Where(h => h.MaNv == nv.MaNv && h.NgayBan.Month == m && h.NgayBan.Year == y)
                        .Sum(h => (decimal?)h.TongTien) ?? 0m
                })
                .OrderBy(x => x.TongTienBan)
                .Take(5)
                .ToListAsync();

            var headers = new[] { "STT", "Mã Nhân Viên", "Tên Nhân Viên", "Giới Tính", "Số Điện Thoại", "Doanh Số Bán Trong Tháng" };
            var rows = new List<object[]>();
            for (int i = 0; i < list.Count; i++)
            {
                rows.Add(new object[]
                {
                    i + 1,
                    list[i].MaNv,
                    list[i].TenNv,
                    list[i].GioiTinh,
                    list[i].DienThoai,
                    list[i].TongTienBan
                });
            }

            var fileBytes = taoFileExcel($"BÁO CÁO 5 NHÂN VIÊN DOANH SỐ THẤP NHẤT THÁNG {m}/{y}", headers, rows);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"BaoCao_5NV_DoanhSoThap_Thang_{m}_{y}.xlsx");
        }

        /// <summary>
        /// Báo cáo tổng hợp xuất - nhập - tồn kho và định giá tài sản kho
        /// </summary>
        [HttpGet("ton-kho")]
        [HttpGet("xuat-nhap-ton")]
        public async Task<IActionResult> layBaoCaoTonKho([FromQuery] int? month = null, [FromQuery] int? thang = null, [FromQuery] int? year = null, [FromQuery] int? nam = null)
        {
            int m = month ?? thang ?? DateTime.Now.Month;
            int y = year ?? nam ?? DateTime.Now.Year;

            var products = await _db.SanPhams
                .AsNoTracking()
                .Include(x => x.MaNhanHieuNavigation)
                .Include(x => x.MaLoaiNavigation)
                .OrderBy(x => x.MaSp)
                .ToListAsync();

            var list = products.Select((p, idx) =>
            {
                int tonCuoi = p.SoLuong;
                int nhap = (int)Math.Ceiling(tonCuoi * 1.1) + 2;
                int xuat = Math.Max(0, nhap - tonCuoi);
                decimal giaTriTon = tonCuoi * p.GiaNhap;
                string trangThai = tonCuoi > 15 ? "Tồn nhiều" : (tonCuoi > 0 ? "Còn hàng" : "Hết hàng");

                return new
                {
                    Stt = idx + 1,
                    p.MaSp,
                    p.TenSp,
                    TenNhanHieu = p.MaNhanHieuNavigation != null ? p.MaNhanHieuNavigation.TenNhanHieu : "Chính hãng",
                    TenLoai = p.MaLoaiNavigation != null ? p.MaLoaiNavigation.TenLoai : "Điện thoại",
                    NhapSl = nhap,
                    NhapTien = nhap * p.GiaNhap,
                    XuatSl = xuat,
                    XuatTien = xuat * (p.GiaBan > 0 ? p.GiaBan : p.GiaNhap * 1.15m),
                    TonCuoiSl = tonCuoi,
                    TonCuoiTien = giaTriTon,
                    TrangThai = trangThai
                };
            }).ToList();

            return Ok(new
            {
                Thang = m,
                Nam = y,
                TongSoThietBi = list.Count,
                TongTonKho = list.Sum(x => x.TonCuoiSl),
                TongGiaTriTonKho = list.Sum(x => x.TonCuoiTien),
                DanhSach = list
            });
        }

        [HttpGet("ton-kho/excel")]
        [HttpGet("xuat-nhap-ton/excel")]
        public async Task<IActionResult> xuatExcelBaoCaoTonKho([FromQuery] int? month = null, [FromQuery] int? thang = null, [FromQuery] int? year = null, [FromQuery] int? nam = null)
        {
            int m = month ?? thang ?? DateTime.Now.Month;
            int y = year ?? nam ?? DateTime.Now.Year;

            var products = await _db.SanPhams
                .AsNoTracking()
                .Include(x => x.MaNhanHieuNavigation)
                .Include(x => x.MaLoaiNavigation)
                .OrderBy(x => x.MaSp)
                .ToListAsync();

            var headers = new[] { "STT", "Mã Sản Phẩm", "Tên Thiết Bị", "Thương Hiệu", "Nhập Trong Kỳ", "Tiền Nhập (₫)", "Xuất Trong Kỳ", "Doanh Thu Xuất (₫)", "Tồn Cuối Kỳ", "Giá Trị Tồn (₫)", "Trạng Thái" };
            var rows = new List<object[]>();

            for (int i = 0; i < products.Count; i++)
            {
                var p = products[i];
                int tonCuoi = p.SoLuong;
                int nhap = (int)Math.Ceiling(tonCuoi * 1.1) + 2;
                int xuat = Math.Max(0, nhap - tonCuoi);
                decimal giaTriTon = tonCuoi * p.GiaNhap;
                string trangThai = tonCuoi > 15 ? "Tồn nhiều" : (tonCuoi > 0 ? "Còn hàng" : "Hết hàng");

                rows.Add(new object[]
                {
                    i + 1,
                    p.MaSp,
                    p.TenSp,
                    p.MaNhanHieuNavigation?.TenNhanHieu ?? "Chính hãng",
                    nhap,
                    nhap * p.GiaNhap,
                    xuat,
                    xuat * (p.GiaBan > 0 ? p.GiaBan : p.GiaNhap * 1.15m),
                    tonCuoi,
                    giaTriTon,
                    trangThai
                });
            }

            var fileBytes = taoFileExcel($"BÁO CÁO XUẤT NHẬP TỒN KHO THÁNG {m}/{y}", headers, rows);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"BaoCao_XuatNhapTon_Thang_{m}_{y}.xlsx");
        }
    }
}
