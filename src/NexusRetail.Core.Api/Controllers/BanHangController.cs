using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusRetail.Core.Api.Data;
using NexusRetail.Shared.DTOs.BanHang;
using NexusRetail.Shared.Entities;

namespace NexusRetail.Core.Api.Controllers
{
    [Route("api/ban-hang")]
    [ApiController]
    public class BanHangController : ControllerBase
    {
        private readonly RetailDbContext _db;

        public BanHangController(RetailDbContext db) { this._db = db; }

        private async Task<string> sinhMaKhachHang()
        {
            var allCodes = await _db.KhachHangs
                .Where(x => x.MaKhach.StartsWith("KH"))
                .Select(x => x.MaKhach)
                .ToListAsync();

            int maxNumber = 0;
            foreach (var code in allCodes)
            {
                var match = System.Text.RegularExpressions.Regex.Match(code, @"^KH(\d+)$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (match.Success && int.TryParse(match.Groups[1].Value, out int num))
                {
                    if (num < 10000 && num > maxNumber)
                    {
                        maxNumber = num;
                    }
                }
            }

            int nextNumber = maxNumber + 1;
            return $"KH{nextNumber:D2}";
        }

        private async Task<string> sinhMaHdb()
        {
            var allCodes = await _db.HoaDonBans
                .Where(x => x.MaHdb.StartsWith("HDB"))
                .Select(x => x.MaHdb)
                .ToListAsync();

            int maxNumber = 0;
            foreach (var code in allCodes)
            {
                var match = System.Text.RegularExpressions.Regex.Match(code, @"^HDB(\d+)$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (match.Success && int.TryParse(match.Groups[1].Value, out int num))
                {
                    if (num < 10000 && num > maxNumber)
                    {
                        maxNumber = num;
                    }
                }
            }

            int nextNumber = maxNumber + 1;
            return $"HDB{nextNumber:D2}";
        }

        /// <summary>
        /// Lấy mã hóa đơn bán tiếp theo theo thứ tự tăng dần chuẩn (Ví dụ: HDB12)
        /// </summary>
        [HttpGet("next-ma-hdb")]
        public async Task<IActionResult> GetNextMaHdb()
        {
            var nextCode = await sinhMaHdb();
            return Ok(new { maHdb = nextCode });
        }

        [HttpPost("thanh-toan")]
        public async Task<IActionResult> thanhToan([FromBody] HoaDonBanDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (dto.DanhSachMon == null || dto.DanhSachMon.Count == 0)
            {
                return BadRequest(new { message = "Hóa đơn bán hàng phải có ít nhất 1 sản phẩm!" });
            }

            if (dto.DanhSachMon.Any(x => x.SoLuong <= 0))
            {
                return BadRequest(new { message = "Số lượng mua của từng sản phẩm phải lớn hơn 0!" });
            }

            if (dto.DanhSachMon.Any(x => x.KhuyenMai < 0 || x.KhuyenMai > 100))
            {
                return BadRequest(new { message = "Mức chiết khấu / khuyến mãi phải từ 0% đến 100%!" });
            }

            using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                var sdtKhach = dto.DienThoai?.Trim() ?? "";
                var maKhach = dto.MaKhach?.Trim();

                var khachHang = await _db.KhachHangs.FirstOrDefaultAsync(x =>
                    (!string.IsNullOrEmpty(sdtKhach) && x.DienThoai == sdtKhach) ||
                    (!string.IsNullOrEmpty(maKhach) && x.MaKhach == maKhach));

                if (khachHang == null)
                {
                    khachHang = new KhachHang
                    {
                        MaKhach = await sinhMaKhachHang(),
                        TenKhach = string.IsNullOrWhiteSpace(dto.TenKhachVangLai) ? "Khách Mua Online" : dto.TenKhachVangLai.Trim(),
                        DienThoai = string.IsNullOrWhiteSpace(sdtKhach) ? "0900000000" : sdtKhach,
                        DiaChi = "Việt Nam"
                    };

                    _db.KhachHangs.Add(khachHang);
                    await _db.SaveChangesAsync();
                }

                // Kiểm tra hợp lệ nhân viên lập hóa đơn (tránh lỗi FK_HoaDonBan_NhanVien)
                var nvHopLe = await _db.NhanViens.AnyAsync(x => x.MaNv == dto.MaNv);
                var maNv = nvHopLe ? dto.MaNv : (await _db.NhanViens.Select(x => x.MaNv).FirstOrDefaultAsync() ?? "NV01");

                string maHdb;
                if (!string.IsNullOrWhiteSpace(dto.MaHdb))
                {
                    var reqMa = dto.MaHdb.Trim().ToUpper();
                    // Nếu mã chưa tồn tại trong DB thì dùng luôn để khớp với mã QR giao dịch ngân hàng
                    bool daTonTai = await _db.HoaDonBans.AnyAsync(x => x.MaHdb == reqMa);
                    maHdb = daTonTai ? await sinhMaHdb() : reqMa;
                }
                else
                {
                    maHdb = await sinhMaHdb();
                }
                decimal tongTienHoaDon = 0m;
                var chiTietList = new List<ChiTietHdb>();

                foreach (var i in dto.DanhSachMon)
                {
                    var sp = await _db.SanPhams.FirstOrDefaultAsync(x => x.MaSp == i.MaSp);
                    if (sp == null)
                    {
                        await tx.RollbackAsync();
                        return BadRequest(new { message = $"Không tìm thấy sản phẩm có mã {i.MaSp}" });
                    }

                    if (sp.SoLuong < i.SoLuong)
                    {
                        await tx.RollbackAsync();
                        return BadRequest(new { message = $"Sản phẩm '{sp.TenSp}' chỉ còn {sp.SoLuong} chiếc trong kho, không đủ để bán!" });
                    }

                    // Nếu đang chạy InMemory (Unit Test không có SQL Server Trigger), cập nhật trực tiếp trong bộ nhớ
                    if (_db.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
                    {
                        sp.SoLuong -= i.SoLuong;
                    }
                    
                    var thanhTien = i.SoLuong * sp.GiaBan * (1 - (i.KhuyenMai / 100m));
                    var chiTietHdb = new ChiTietHdb
                    {
                        MaHdb = maHdb,
                        MaSp = i.MaSp,
                        SoLuong = i.SoLuong,
                        KhuyenMai = i.KhuyenMai,
                        ThanhTien = thanhTien
                    };
                    tongTienHoaDon += thanhTien;
                    _db.ChiTietHdbs.Add(chiTietHdb);
                    chiTietList.Add(chiTietHdb);
                }

                var hdb = new HoaDonBan
                {
                    MaHdb = maHdb,
                    MaNv = maNv,
                    NgayBan = DateTime.Now,
                    MaKhach = khachHang.MaKhach,
                    TongTien = tongTienHoaDon
                };

                _db.HoaDonBans.Add(hdb);
                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                var response = new HoaDonBanResponseDto
                {
                    MaHdb = maHdb,
                    MaNv = maNv,
                    TenNv = await _db.NhanViens.Where(x => x.MaNv == maNv).Select(x => x.TenNv).FirstOrDefaultAsync() ?? "",
                    NgayBan = hdb.NgayBan,
                    TongTien = tongTienHoaDon,
                    SanPham = dto.DanhSachMon.Select(m => new ChiTietHdbDto
                    {
                        MaSp = m.MaSp,
                        TenSp = _db.SanPhams.Where(x => x.MaSp == m.MaSp).Select(x => x.TenSp).FirstOrDefault() ?? "",
                        SoLuong = m.SoLuong,
                        KhuyenMai = m.KhuyenMai
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                var loi = ex.GetBaseException().Message;
                return StatusCode(500, new { message = loi });
            }
        }

        /// <summary>
        /// Tra cứu lịch sử đơn hàng và hạn bảo hành theo số điện thoại
        /// </summary>
        [HttpGet("tra-cuu/{sdt}")]
        public async Task<IActionResult> traCuuDonHang(string sdt)
        {
            if (string.IsNullOrWhiteSpace(sdt))
                return BadRequest(new { message = "Số điện thoại không được để trống!" });

            sdt = sdt.Trim();

            var orders = await _db.HoaDonBans
                .AsNoTracking()
                .Include(x => x.MaKhachNavigation)
                .Include(x => x.ChiTietHdbs)
                    .ThenInclude(ct => ct.MaSpNavigation)
                .Where(x => x.MaKhachNavigation != null && x.MaKhachNavigation.DienThoai == sdt)
                .OrderByDescending(x => x.NgayBan)
                .ToListAsync();

            var list = orders.Select(x => new
            {
                x.MaHdb,
                x.NgayBan,
                x.TongTien,
                TenKhach = x.MaKhachNavigation?.TenKhach ?? "",
                DienThoai = x.MaKhachNavigation?.DienThoai ?? "",
                DanhSachMon = x.ChiTietHdbs.Select(ct => new
                {
                    ct.MaSp,
                    TenSp = ct.MaSpNavigation?.TenSp ?? ct.MaSp,
                    ct.SoLuong,
                    ct.ThanhTien,
                    ThoiGianBaoHanh = ct.MaSpNavigation?.ThoiGianBaoHanh ?? 12,
                    HanBaoHanh = x.NgayBan.AddMonths(ct.MaSpNavigation?.ThoiGianBaoHanh ?? 12)
                }).ToList()
            }).ToList();

            return Ok(list);
        }

        /// <summary>
        /// Lấy danh sách hóa đơn bán hàng cho Quản trị viên (có phân trang & tìm kiếm)
        /// </summary>
        [HttpGet("danh-sach")]
        public async Task<IActionResult> layDanhSach(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 15,
            [FromQuery] string? q = null)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _db.HoaDonBans
                .AsNoTracking()
                .Include(x => x.MaKhachNavigation)
                .Include(x => x.MaNvNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(x =>
                    x.MaHdb.Contains(q) ||
                    (x.MaKhachNavigation != null && (x.MaKhachNavigation.TenKhach.Contains(q) || x.MaKhachNavigation.DienThoai.Contains(q))) ||
                    (x.MaNvNavigation != null && x.MaNvNavigation.TenNv.Contains(q)));
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var items = await query
                .OrderByDescending(x => x.NgayBan)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    maHdb = x.MaHdb,
                    ngayBan = x.NgayBan,
                    tongTien = x.TongTien,
                    tenKhach = x.MaKhachNavigation != null ? x.MaKhachNavigation.TenKhach : "Khách Lẻ",
                    dienThoai = x.MaKhachNavigation != null ? x.MaKhachNavigation.DienThoai : "",
                    tenNv = x.MaNvNavigation != null ? x.MaNvNavigation.TenNv : "Khách Online"
                })
                .ToListAsync();

            return Ok(new
            {
                page,
                pageSize,
                totalItems,
                totalPages,
                items
            });
        }

        /// <summary>
        /// Lấy chi tiết một hóa đơn bán hàng theo mã HDB
        /// </summary>
        [HttpGet("chi-tiet/{id}")]
        public async Task<IActionResult> layChiTiet(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new { message = "Mã hóa đơn không được để trống!" });

            var hdb = await _db.HoaDonBans
                .AsNoTracking()
                .Include(x => x.MaKhachNavigation)
                .Include(x => x.MaNvNavigation)
                .Include(x => x.ChiTietHdbs)
                    .ThenInclude(ct => ct.MaSpNavigation)
                .FirstOrDefaultAsync(x => x.MaHdb == id);

            if (hdb == null)
                return NotFound(new { message = $"Không tìm thấy hóa đơn {id}" });

            return Ok(new
            {
                maHdb = hdb.MaHdb,
                ngayBan = hdb.NgayBan,
                tongTien = hdb.TongTien,
                tenKhach = hdb.MaKhachNavigation?.TenKhach ?? "Khách Lẻ",
                dienThoai = hdb.MaKhachNavigation?.DienThoai ?? "",
                tenNv = hdb.MaNvNavigation?.TenNv ?? "Khách Online",
                chiTiet = hdb.ChiTietHdbs.Select(ct => new
                {
                    maSp = ct.MaSp,
                    tenSp = ct.MaSpNavigation?.TenSp ?? ct.MaSp,
                    soLuong = ct.SoLuong,
                    donGia = ct.MaSpNavigation != null ? ct.MaSpNavigation.GiaBan : (ct.SoLuong > 0 ? ct.ThanhTien / ct.SoLuong : 0),
                    khuyenMai = ct.KhuyenMai,
                    thanhTien = ct.ThanhTien
                }).ToList()
            });
        }

        /// <summary>
        /// Thống kê tổng quan cho Dashboard Quản trị viên
        /// </summary>
        [HttpGet("thong-ke-dashboard")]
        public async Task<IActionResult> layThongKeDashboard()
        {
            var totalProducts = await _db.SanPhams.CountAsync();
            var totalOrders = await _db.HoaDonBans.CountAsync();
            var totalRevenue = await _db.HoaDonBans.SumAsync(x => (decimal?)x.TongTien) ?? 0m;
            var totalCustomers = await _db.KhachHangs.CountAsync();

            return Ok(new
            {
                totalProducts,
                totalOrders,
                totalRevenue,
                totalCustomers
            });
        }
    }
}
