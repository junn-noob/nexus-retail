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
                var khachHang = _db.KhachHangs.FirstOrDefault(x => (x.DienThoai.Equals(dto.DienThoai)) || (x.MaKhach == dto.MaKhach));
                if (dto.MaKhach != null && khachHang != null)
                {
                    dto.DienThoai = khachHang.DienThoai;
                } 
                else if (dto.DienThoai != null && khachHang != null)
                {
                    dto.MaKhach = khachHang.MaKhach;
                }
                else
                {
                    khachHang = new KhachHang
                    {
                        MaKhach = await sinhMaKhachHang(),
                        TenKhach = dto.TenKhachVangLai ?? "",
                        DienThoai = dto.DienThoai ?? "",
                    };

                    _db.KhachHangs.Add(khachHang);
                }

                var maHdb = await sinhMaHdb();
                decimal tongTienHoaDon = 0m;
                foreach (var i in dto.DanhSachMon)
                {
                    var sp = await _db.SanPhams.FindAsync(i.MaSp);
                    if (sp == null)
                    {
                        await tx.RollbackAsync();

                        return BadRequest($"Không tìm thấy sản phẩm có mã {i.MaSp}");
                    }

                    if (sp.SoLuong < i.SoLuong)
                    {
                        await tx.RollbackAsync();
                        return BadRequest(new { message = $"Sản phẩm '{sp.TenSp}' chỉ còn {sp.SoLuong} chiếc trong kho, không đủ để bán!" });
                    }
                    
                    sp.SoLuong -= i.SoLuong; // BR-01: Trừ tồn kho khi xuất bán
                    var chiTietHdb = new ChiTietHdb
                    {
                        MaHdb = maHdb,
                        MaSp = i.MaSp,
                        SoLuong = i.SoLuong,
                        KhuyenMai = i.KhuyenMai,
                        ThanhTien = i.SoLuong * sp.GiaBan * (1 - i.KhuyenMai / 100m)
                    };
                    tongTienHoaDon += chiTietHdb.ThanhTien;
                    _db.ChiTietHdbs.Add(chiTietHdb);
                }

                var hdb = new HoaDonBan
                {
                    MaHdb = maHdb,
                    MaNv = dto.MaNv,
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
                    MaNv = dto.MaNv,
                    TenNv = await _db.NhanViens.Where(x => x.MaNv == dto.MaNv).Select(x => x.TenNv).FirstOrDefaultAsync() ?? "",
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

                return StatusCode(500, new
                {
                    message = loi
                });
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

            var list = await _db.HoaDonBans
                .AsNoTracking()
                .Include(x => x.MaKhachNavigation)
                .Include(x => x.ChiTietHdbs).ThenInclude(ct => ct.MaSpNavigation)
                .Where(x => x.MaKhachNavigation.DienThoai == sdt)
                .OrderByDescending(x => x.NgayBan)
                .Select(x => new
                {
                    x.MaHdb,
                    x.NgayBan,
                    x.TongTien,
                    TenKhach = x.MaKhachNavigation.TenKhach,
                    DienThoai = x.MaKhachNavigation.DienThoai,
                    DanhSachMon = x.ChiTietHdbs.Select(ct => new
                    {
                        ct.MaSp,
                        TenSp = ct.MaSpNavigation.TenSp,
                        ct.SoLuong,
                        ct.ThanhTien,
                        ThoiGianBaoHanh = ct.MaSpNavigation.ThoiGianBaoHanh,
                        HanBaoHanh = x.NgayBan.AddMonths(ct.MaSpNavigation.ThoiGianBaoHanh)
                    }).ToList()
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("danh-sach")]
        public async Task<IActionResult> layDanhSachDonHang([FromQuery] int page = 1, [FromQuery] int pageSize = 15)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            var query = _db.HoaDonBans
                .AsNoTracking()
                .Include(x => x.MaKhachNavigation)
                .Include(x => x.MaNvNavigation)
                .OrderByDescending(x => x.NgayBan);

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new
                {
                    x.MaHdb,
                    x.NgayBan,
                    x.TongTien,
                    TenKhach = x.MaKhachNavigation.TenKhach,
                    DienThoai = x.MaKhachNavigation.DienThoai,
                    TenNv = x.MaNvNavigation.TenNv
                })
                .ToListAsync();

            return Ok(new
            {
                page,
                pageSize,
                totalItems,
                totalPages = (int)Math.Ceiling((double)totalItems / pageSize),
                items
            });
        }

        [HttpGet("chi-tiet/{maHdb}")]
        public async Task<IActionResult> chiTietDonHang(string maHdb)
        {
            var order = await _db.HoaDonBans
                .AsNoTracking()
                .Include(x => x.MaKhachNavigation)
                .Include(x => x.MaNvNavigation)
                .Include(x => x.ChiTietHdbs).ThenInclude(ct => ct.MaSpNavigation)
                .FirstOrDefaultAsync(x => x.MaHdb == maHdb);

            if (order == null) return NotFound(new { message = "Không tìm thấy đơn hàng!" });

            return Ok(new
            {
                order.MaHdb,
                order.NgayBan,
                order.TongTien,
                TenKhach = order.MaKhachNavigation?.TenKhach ?? "Khách vãng lai",
                DienThoai = order.MaKhachNavigation?.DienThoai ?? "",
                TenNv = order.MaNvNavigation?.TenNv ?? "Hệ thống Online",
                ChiTiet = order.ChiTietHdbs.Select(ct => new
                {
                    ct.MaSp,
                    TenSp = ct.MaSpNavigation != null ? ct.MaSpNavigation.TenSp : "",
                    ct.SoLuong,
                    ct.ThanhTien,
                    ct.KhuyenMai,
                    DonGia = ct.MaSpNavigation != null ? ct.MaSpNavigation.GiaBan : 0
                }).ToList()
            });
        }
    }
}
