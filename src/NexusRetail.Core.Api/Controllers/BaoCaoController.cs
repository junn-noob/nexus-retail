using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusRetail.Core.Api.Data;

namespace NexusRetail.Core.Api.Controllers
{
    [Route("api/bao-cao")]
    [ApiController]
    public class BaoCaoController : ControllerBase
    {
        private readonly RetailDbContext _db;

        public BaoCaoController(RetailDbContext db)
        {
            _db = db;
        }

        #region Helper Xuất Excel dùng ClosedXML
        private byte[] taoFileExcel(string tieuDe, string[] cotTieuDe, List<object[]> duLieuHang)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("BaoCao");

            // 1. Tiêu đề chính lớn
            worksheet.Cell(1, 1).Value = tieuDe;
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Cell(1, 1).Style.Font.FontColor = XLColor.DarkBlue;
            worksheet.Range(1, 1, 1, cotTieuDe.Length).Merge();
            worksheet.Row(1).Height = 30;
            worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(1, 1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            // 2. Ngày xuất báo cáo
            worksheet.Cell(2, 1).Value = $"Thời gian xuất báo cáo: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
            worksheet.Cell(2, 1).Style.Font.Italic = true;
            worksheet.Range(2, 1, 2, cotTieuDe.Length).Merge();

            // 3. Header cột
            for (int i = 0; i < cotTieuDe.Length; i++)
            {
                var cell = worksheet.Cell(4, i + 1);
                cell.Value = cotTieuDe[i];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1E3A8A"); // Navy blue
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }
            worksheet.Row(4).Height = 25;

            // 4. Dữ liệu từng dòng
            for (int r = 0; r < duLieuHang.Count; r++)
            {
                var rowNumber = r + 5;
                for (int c = 0; c < cotTieuDe.Length; c++)
                {
                    var cell = worksheet.Cell(rowNumber, c + 1);
                    var val = duLieuHang[r][c];

                    if (val is decimal || val is double || val is int || val is long)
                    {
                        cell.Value = Convert.ToDouble(val);
                        if (cotTieuDe[c].Contains("Tiền") || cotTieuDe[c].Contains("Thu") || cotTieuDe[c].Contains("Giá"))
                        {
                            cell.Style.NumberFormat.Format = "#,##0 ₫";
                        }
                    }
                    else
                    {
                        cell.Value = val?.ToString() ?? "";
                    }

                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.OutsideBorderColor = XLColor.LightGray;
                }
            }

            worksheet.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
        #endregion

        // =========================================================================
        // BR-06: BÁO CÁO TOP 5 SẢN PHẨM BÁN CHẠY NHẤT THEO THÁNG / NĂM
        // =========================================================================
        [HttpGet("sp-ban-chay")]
        public async Task<IActionResult> laySpBanChay([FromQuery] int month, [FromQuery] int year)
        {
            if (month < 1 || month > 12 || year < 2000)
                return BadRequest(new { message = "Tháng (1-12) hoặc năm không hợp lệ!" });

            var list = await _db.ChiTietHdbs
                .AsNoTracking()
                .Include(x => x.MaHdbNavigation)
                .Include(x => x.MaSpNavigation)
                .Where(x => x.MaHdbNavigation.NgayBan.Month == month && x.MaHdbNavigation.NgayBan.Year == year)
                .GroupBy(x => new { x.MaSp, TenSp = x.MaSpNavigation.TenSp })
                .Select(g => new
                {
                    MaSp = g.Key.MaSp,
                    TenSp = g.Key.TenSp,
                    TongSoLuong = g.Sum(x => x.SoLuong),
                    TongDoanhThu = g.Sum(x => x.ThanhTien)
                })
                .OrderByDescending(x => x.TongSoLuong)
                .Take(5)
                .ToListAsync();

            return Ok(new
            {
                Thang = month,
                Nam = year,
                TopSanPham = list
            });
        }

        [HttpGet("sp-ban-chay/excel")]
        public async Task<IActionResult> xuatExcelSpBanChay([FromQuery] int month, [FromQuery] int year)
        {
            var data = await _db.ChiTietHdbs
                .AsNoTracking()
                .Include(x => x.MaHdbNavigation)
                .Include(x => x.MaSpNavigation)
                .Where(x => x.MaHdbNavigation.NgayBan.Month == month && x.MaHdbNavigation.NgayBan.Year == year)
                .GroupBy(x => new { x.MaSp, TenSp = x.MaSpNavigation.TenSp })
                .Select(g => new
                {
                    MaSp = g.Key.MaSp,
                    TenSp = g.Key.TenSp,
                    TongSoLuong = g.Sum(x => x.SoLuong),
                    TongDoanhThu = g.Sum(x => x.ThanhTien)
                })
                .OrderByDescending(x => x.TongSoLuong)
                .Take(5)
                .ToListAsync();

            var headers = new[] { "STT", "Mã Sản Phẩm", "Tên Sản Phẩm", "Số Lượng Bán", "Tổng Doanh Thu" };
            var rows = new List<object[]>();
            for (int i = 0; i < data.Count; i++)
            {
                rows.Add(new object[]
                {
                    i + 1,
                    data[i].MaSp,
                    data[i].TenSp,
                    data[i].TongSoLuong,
                    data[i].TongDoanhThu
                });
            }

            var fileBytes = taoFileExcel($"BÁO CÁO TOP 5 SẢN PHẨM BÁN CHẠY NHẤT THÁNG {month}/{year}", headers, rows);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"BaoCao_Top5_SanPham_Thang_{month}_{year}.xlsx");
        }

        // =========================================================================
        // BR-07: BÁO CÁO HÓA ĐƠN NHẬP LỚN NHẤT THEO QUÝ / NĂM
        // =========================================================================
        [HttpGet("hdn-lon-nhat")]
        public async Task<IActionResult> layHdnLonNhat([FromQuery] int quarter, [FromQuery] int year)
        {
            if (quarter < 1 || quarter > 4 || year < 2000)
                return BadRequest(new { message = "Quý (1-4) hoặc năm không hợp lệ!" });

            int startMonth = (quarter - 1) * 3 + 1;
            int endMonth = startMonth + 2;

            var list = await _db.HoaDonNhaps
                .AsNoTracking()
                .Include(x => x.MaNccNavigation)
                .Include(x => x.MaNvNavigation)
                .Where(x => x.NgayNhap.Year == year && x.NgayNhap.Month >= startMonth && x.NgayNhap.Month <= endMonth)
                .OrderByDescending(x => x.TongTien)
                .Select(x => new
                {
                    x.MaHdn,
                    x.NgayNhap,
                    x.MaNcc,
                    TenNcc = x.MaNccNavigation.TenNcc,
                    x.MaNv,
                    TenNv = x.MaNvNavigation.TenNv,
                    x.TongTien
                })
                .ToListAsync();

            return Ok(new
            {
                Quy = quarter,
                Nam = year,
                TongTienNhapQuy = list.Sum(x => x.TongTien),
                HoaDonLonNhat = list.FirstOrDefault(),
                DanhSachHoaDon = list
            });
        }

        [HttpGet("hdn-lon-nhat/excel")]
        public async Task<IActionResult> xuatExcelHdnLonNhat([FromQuery] int quarter, [FromQuery] int year)
        {
            int startMonth = (quarter - 1) * 3 + 1;
            int endMonth = startMonth + 2;

            var list = await _db.HoaDonNhaps
                .AsNoTracking()
                .Include(x => x.MaNccNavigation)
                .Include(x => x.MaNvNavigation)
                .Where(x => x.NgayNhap.Year == year && x.NgayNhap.Month >= startMonth && x.NgayNhap.Month <= endMonth)
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

            var fileBytes = taoFileExcel($"BÁO CÁO HÓA ĐƠN NHẬP LỚN NHẤT QUÝ {quarter} NĂM {year}", headers, rows);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"BaoCao_HDN_LonNhat_Quy_{quarter}_{year}.xlsx");
        }

        // =========================================================================
        // BR-08: BÁO CÁO DOANH SỐ BÁN THEO NĂM
        // =========================================================================
        [HttpGet("doanh-so-nam")]
        public async Task<IActionResult> layDoanhSoNam([FromQuery] int year)
        {
            if (year < 2000)
                return BadRequest(new { message = "Năm không hợp lệ!" });

            var list = await _db.HoaDonBans
                .AsNoTracking()
                .Include(x => x.MaKhachNavigation)
                .Include(x => x.MaNvNavigation)
                .Where(x => x.NgayBan.Year == year)
                .OrderBy(x => x.NgayBan)
                .Select(x => new
                {
                    x.MaHdb,
                    x.NgayBan,
                    x.MaKhach,
                    TenKhach = x.MaKhachNavigation.TenKhach,
                    x.MaNv,
                    TenNv = x.MaNvNavigation.TenNv,
                    x.TongTien
                })
                .ToListAsync();

            return Ok(new
            {
                Nam = year,
                TongDoanhThu = list.Sum(x => x.TongTien),
                TongSoDonHang = list.Count,
                DanhSachHoaDon = list
            });
        }

        [HttpGet("doanh-so-nam/excel")]
        public async Task<IActionResult> xuatExcelDoanhSoNam([FromQuery] int year)
        {
            var list = await _db.HoaDonBans
                .AsNoTracking()
                .Include(x => x.MaKhachNavigation)
                .Include(x => x.MaNvNavigation)
                .Where(x => x.NgayBan.Year == year)
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

            var fileBytes = taoFileExcel($"BÁO CÁO TỔNG KẾT DOANH SỐ BÁN NĂM {year}", headers, rows);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"BaoCao_DoanhSo_Nam_{year}.xlsx");
        }

        // =========================================================================
        // BR-09: TOP 5 NHÂN VIÊN CÓ DOANH SỐ THẤP NHẤT TRONG THÁNG
        // =========================================================================
        [HttpGet("nv-doanh-so-thap")]
        public async Task<IActionResult> layNvDoanhSoThap([FromQuery] int month, [FromQuery] int year)
        {
            if (month < 1 || month > 12 || year < 2000)
                return BadRequest(new { message = "Tháng hoặc năm không hợp lệ!" });

            var list = await _db.NhanViens
                .AsNoTracking()
                .Select(nv => new
                {
                    nv.MaNv,
                    nv.TenNv,
                    nv.DienThoai,
                    TongTienBan = _db.HoaDonBans
                        .Where(h => h.MaNv == nv.MaNv && h.NgayBan.Month == month && h.NgayBan.Year == year)
                        .Sum(h => (decimal?)h.TongTien) ?? 0m
                })
                .OrderBy(x => x.TongTienBan)
                .Take(5)
                .ToListAsync();

            return Ok(new
            {
                Thang = month,
                Nam = year,
                DanhSachNhanVien = list
            });
        }

        [HttpGet("nv-doanh-so-thap/excel")]
        public async Task<IActionResult> xuatExcelNvDoanhSoThap([FromQuery] int month, [FromQuery] int year)
        {
            var list = await _db.NhanViens
                .AsNoTracking()
                .Select(nv => new
                {
                    nv.MaNv,
                    nv.TenNv,
                    nv.DienThoai,
                    TongTienBan = _db.HoaDonBans
                        .Where(h => h.MaNv == nv.MaNv && h.NgayBan.Month == month && h.NgayBan.Year == year)
                        .Sum(h => (decimal?)h.TongTien) ?? 0m
                })
                .OrderBy(x => x.TongTienBan)
                .Take(5)
                .ToListAsync();

            var headers = new[] { "STT", "Mã Nhân Viên", "Tên Nhân Viên", "Số Điện Thoại", "Doanh Số Bán Trong Tháng" };
            var rows = new List<object[]>();
            for (int i = 0; i < list.Count; i++)
            {
                rows.Add(new object[]
                {
                    i + 1,
                    list[i].MaNv,
                    list[i].TenNv,
                    list[i].DienThoai ?? "",
                    list[i].TongTienBan
                });
            }

            var fileBytes = taoFileExcel($"BÁO CÁO 5 NHÂN VIÊN DOANH SỐ THẤP NHẤT THÁNG {month}/{year}", headers, rows);
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"BaoCao_5NV_DoanhSoThap_Thang_{month}_{year}.xlsx");
        }
    }
}
