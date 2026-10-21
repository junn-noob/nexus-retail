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
            var maCuoi = await _db.KhachHangs
                .Where(x => x.MaKhach.StartsWith("KH"))
                .OrderByDescending(x => x.MaKhach)
                .Select(x => x.MaKhach)
                .FirstOrDefaultAsync();

            if (maCuoi == null) return "KH01";

            if (int.TryParse(maCuoi.Substring(2), out int soHienTai))
                return $"KH{(soHienTai + 1):D2}";

            return $"KH{Guid.NewGuid().ToString()[..6].ToUpper()}";
        }

        private async Task<string> sinhMaHdb()
        {
            var maCuoi = await _db.HoaDonBans
                .Where(x => x.MaHdb.StartsWith("HDB"))
                .OrderByDescending(x => x.MaHdb)
                .Select(x => x.MaHdb)
                .FirstOrDefaultAsync();

            if (maCuoi == null) return "HDB01";

            if (int.TryParse(maCuoi.Substring(3), out int soHienTai))
                return $"HDB{(soHienTai + 1):D2}";

            return $"HDB{Guid.NewGuid().ToString()[..6].ToUpper()}";
        }

        [HttpPost("thanh-toan")]
        public async Task<IActionResult> thanhToan([FromBody] HoaDonBanDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

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

                var maHdb = await sinhMaHdb();
                decimal tongTienHoaDon = 0m;
                var chiTietList = new List<ChiTietHdb>();

                foreach (var i in dto.DanhSachMon)
                {
                    var sp = await _db.SanPhams.AsNoTracking().FirstOrDefaultAsync(x => x.MaSp == i.MaSp);
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
    }
}
