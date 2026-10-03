using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusRetail.Core.Api.Data;
using NexusRetail.Shared.DTOs.SanPham;
using NexusRetail.Shared.Entities;


namespace NexusRetail.Core.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanPhamAPI : ControllerBase
    {
        private readonly RetailDbContext _db;

        public SanPhamAPI(RetailDbContext db) { this._db = db; }

        [HttpGet]
        public async Task<IActionResult> layTatCa()
        {
            var danhSach = await _db.SanPhams
                .AsNoTracking()
                .ToListAsync();

            return Ok(danhSach);
        }


        [HttpGet("{maSP}")]
        public async Task<IActionResult> chiTietSanPham(string maSP)
        {
            var sanPham = await _db.SanPhams
                .FirstOrDefaultAsync(x => x.MaSp == maSP);

            if (sanPham == null) return NotFound();

            return Ok(sanPham);
        }

        [HttpGet("NhanHieu/{maNhanHieu}")]
        public async Task<IActionResult> laySanPhamTheoNhanHieu(string maNhanHieu)
        {
            var danhSach = await _db.SanPhams.AsNoTracking().Where(x => x.MaNhanHieu == maNhanHieu).ToListAsync();

            if (danhSach == null) return NotFound();

            return Ok(danhSach);
        }

        [HttpGet("search")]
        public async Task<IActionResult> timKiem([FromQuery] string? keyword, [FromQuery] string? maLoai, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        {
            var query = _db.SanPhams.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.TenSp.Contains(keyword) || x.MaSp.Contains(keyword));
            if (!string.IsNullOrEmpty(maLoai))
                query = query.Where(x => x.MaLoai == maLoai);
            if (minPrice.HasValue)
                query = query.Where(x => x.GiaBan >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(x => x.GiaBan <= maxPrice.Value);

            return Ok(await query.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> themSanPham([FromBody] ThemSanPhamDtos dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var sp = new SanPham
            {
                MaSp = dto.MaSp,
                TenSp = dto.TenSp,
                MaLoai = dto.MaLoai,
                MaNhanHieu = dto.MaNhanHieu,
                MaManHinh = dto.MaManHinh,
                GiaNhap = dto.GiaNhap,
                GiaBan = Math.Round(dto.GiaNhap * 1.1m, 0),
                SoLuong = dto.SoLuong,
                ThoiGianBaoHanh = dto.ThoiGianBaoHanh,
                AmThanh = dto.AmThanh,
                ChupAnh = dto.ChupAnh,
                Anh = dto.Anh,
                GhiChu = dto.GhiChu
            };

            _db.SanPhams.Add(sp);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(chiTietSanPham), new { maSP = sp.MaSp }, sp);
        }

        [HttpPut("{maSP}")]
        public async Task<IActionResult> suaSanPham(string maSP, [FromBody] SuaSanPhamDtos dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var sp = await _db.SanPhams
                .FirstOrDefaultAsync(x => x.MaSp == maSP);

            if (sp == null) return NotFound();

            sp.TenSp = dto.TenSp;
            sp.MaLoai = dto.MaLoai;
            sp.MaNhanHieu = dto.MaNhanHieu;
            sp.MaManHinh = dto.MaManHinh;
            sp.GiaNhap = dto.GiaNhap;
            sp.GiaBan = Math.Round(dto.GiaNhap * 1.1m, 0);
            sp.SoLuong = dto.SoLuong;
            sp.ThoiGianBaoHanh = dto.ThoiGianBaoHanh;
            sp.AmThanh = dto.AmThanh;
            sp.ChupAnh = dto.ChupAnh;
            sp.Anh = dto.Anh;
            sp.GhiChu = dto.GhiChu;

            _db.SanPhams.Update(sp);
            await _db.SaveChangesAsync();

            return NoContent(); // 204
        }

        [HttpDelete("{maSP}")]
        public async Task<IActionResult> xoaSanPham(string maSP)
        {
            var sp = await _db.SanPhams.FindAsync(maSP);
            if (sp == null) return NotFound();

            _db.SanPhams.Remove(sp);
            await _db.SaveChangesAsync();

            return NoContent(); // 204
        }

    }
}
