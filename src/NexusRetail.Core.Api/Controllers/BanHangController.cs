using Microsoft.AspNetCore.Http;
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
            // Tìm mã lớn nhất hiện có trong DB dạng "SP___"
            var maCuoi = await _db.KhachHangs
                .Where(x => x.MaKhach.StartsWith("KH"))
                .OrderByDescending(x => x.MaKhach)
                .Select(x => x.MaKhach)
                .FirstOrDefaultAsync();

            if (maCuoi == null) return "KH01"; // DB trống → bắt đầu từ SP01

            // Tách phần số: "SP012" → 12
            if (int.TryParse(maCuoi.Substring(2), out int soHienTai))
                return $"KH{(soHienTai + 1):D2}";

            return $"KH{Guid.NewGuid().ToString()[..6].ToUpper()}"; // fallback nếu parse lỗi
        }

        private async Task<string> sinhMaHdb()
        {
            // Tìm mã lớn nhất hiện có trong DB dạng "SP___"
            var maCuoi = await _db.HoaDonBans
                .Where(x => x.MaHdb.StartsWith("HDB"))
                .OrderByDescending(x => x.MaHdb)
                .Select(x => x.MaHdb)
                .FirstOrDefaultAsync();

            if (maCuoi == null) return "HDB01"; // DB trống → bắt đầu từ SP01

            // Tách phần số: "SP012" → 12
            if (int.TryParse(maCuoi.Substring(3), out int soHienTai))
                return $"HDB{(soHienTai + 1):D2}";

            return $"HDB{Guid.NewGuid().ToString()[..6].ToUpper()}"; // fallback nếu parse lỗi
        }

        [HttpPost("thanh-toan")]
        public async Task<IActionResult> thanhToan([FromBody] HoaDonBanDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                var khachHang = _db.KhachHangs.FirstOrDefault(x => x.MaKhach == dto.MaKhach);
                if (dto.MaKhach != null && khachHang != null)
                {
                    dto.DienThoai = khachHang.DienThoai;
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
                        return BadRequest(new { message = $"Sản phẩm '{sp.TenSp}' chỉ còn {sp.SoLuong} chiếc trong kho, không đủ để bán!" });
                    }
                    
                    sp.SoLuong -= i.SoLuong;
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
            catch (Exception ex) { await tx.RollbackAsync(); return StatusCode(500, ex.Message); }
        }
    }
}
