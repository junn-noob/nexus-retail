using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusRetail.Core.Api.Data;
using NexusRetail.Shared.DTOs.SanPham;
using NexusRetail.Shared.Entities;


namespace NexusRetail.Core.Api.Controllers
{
    [Route("api/san-pham")]
    [ApiController]
    public class SanPhamController : ControllerBase
    {
        private readonly RetailDbContext _db;

        public SanPhamController(RetailDbContext db) { this._db = db; }

        [HttpGet]
        public async Task<IActionResult> layTatCa()
        {
            var danhSach = await _db.VwDanhSachSanPhams
                .AsNoTracking()
                .Select(sp => new SanPhamDto
                {
                    MaSp = sp.MaSp,
                    TenSp = sp.TenSp,
                    TenLoai = sp.TenLoai,
                    MaNhanHieu = sp.MaNhanHieu,
                    TenNhanHieu = sp.TenNhanHieu,
                    TenManHinh = sp.TenManHinh,
                    GiaBan = sp.GiaBan,
                    SoLuong = sp.SoLuong,
                    ThoiGianBaoHanh = sp.ThoiGianBaoHanh,
                    AmThanh = sp.AmThanh,
                    ChupAnh = sp.ChupAnh,
                    Anh = sp.Anh,
                    GhiChu = sp.GhiChu
                })
                .ToListAsync();

            return Ok(danhSach);
        }


        [HttpGet("{maSP}")]
        public async Task<IActionResult> chiTietSanPham(string maSP)
        {
            var sanPham = await _db.VwDanhSachSanPhams
                .AsNoTracking()
                .Select(sp => new SanPhamDto
                {
                    MaSp = sp.MaSp,
                    TenSp = sp.TenSp,
                    TenLoai = sp.TenLoai,
                    MaNhanHieu = sp.MaNhanHieu,
                    TenNhanHieu = sp.TenNhanHieu,
                    TenManHinh = sp.TenManHinh,
                    GiaBan = sp.GiaBan,
                    SoLuong = sp.SoLuong,
                    ThoiGianBaoHanh = sp.ThoiGianBaoHanh,
                    AmThanh = sp.AmThanh,
                    ChupAnh = sp.ChupAnh,
                    Anh = sp.Anh,
                    GhiChu = sp.GhiChu
                })
                .FirstOrDefaultAsync(x => x.MaSp == maSP);

            if (sanPham == null) return NotFound();

            return Ok(sanPham);
        }

        [HttpGet("nhan-hieu/{maNhanHieu}")]
        public async Task<IActionResult> laySanPhamTheoNhanHieu(string maNhanHieu)
        {
            var danhSach = await _db.VwDanhSachSanPhams
                .AsNoTracking()
                .Where(x => x.MaNhanHieu == maNhanHieu)
                .Select(sp => new SanPhamDto
                {
                    MaSp = sp.MaSp,
                    TenSp = sp.TenSp,
                    TenLoai = sp.TenLoai,
                    MaNhanHieu = sp.MaNhanHieu,
                    TenNhanHieu = sp.TenNhanHieu,
                    TenManHinh = sp.TenManHinh,
                    GiaBan = sp.GiaBan,
                    SoLuong = sp.SoLuong,
                    ThoiGianBaoHanh = sp.ThoiGianBaoHanh,
                    AmThanh = sp.AmThanh,
                    ChupAnh = sp.ChupAnh,
                    Anh = sp.Anh,
                    GhiChu = sp.GhiChu
                })
                .ToListAsync();

            if (danhSach.Count == 0) return NotFound();

            return Ok(danhSach);
        }

        [HttpGet("tim-kiem")]
        public async Task<IActionResult> timKiem([FromQuery] string? keyword, [FromQuery] string? tenLoai, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice)
        {
            var query = _db.VwDanhSachSanPhams
                .AsNoTracking()
                .Select(sp => new SanPhamDto
                {
                    MaSp = sp.MaSp,
                    TenSp = sp.TenSp,
                    TenLoai = sp.TenLoai,
                    MaNhanHieu = sp.MaNhanHieu,
                    TenNhanHieu = sp.TenNhanHieu,
                    TenManHinh = sp.TenManHinh,
                    GiaBan = sp.GiaBan,
                    SoLuong = sp.SoLuong,
                    ThoiGianBaoHanh = sp.ThoiGianBaoHanh,
                    AmThanh = sp.AmThanh,
                    ChupAnh = sp.ChupAnh,
                    Anh = sp.Anh,
                    GhiChu = sp.GhiChu
                })
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.TenSp.Contains(keyword) || (x.GhiChu != null && x.GhiChu.Contains(keyword)));
            if (!string.IsNullOrEmpty(tenLoai))
                query = query.Where(x => x.TenLoai.Contains(tenLoai));
            if (minPrice.HasValue)
                query = query.Where(x => x.GiaBan >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(x => x.GiaBan <= maxPrice.Value);

            return Ok(await query.ToListAsync());
        }

        private async Task<string> sinhMaSanPham()
        {
            // Tìm mã lớn nhất hiện có trong DB dạng "SP___"
            var maCuoi = await _db.SanPhams
                .Where(x => x.MaSp.StartsWith("SP"))
                .OrderByDescending(x => x.MaSp)
                .Select(x => x.MaSp)
                .FirstOrDefaultAsync();

            if (maCuoi == null) return "SP001"; // DB trống → bắt đầu từ SP001

            // Tách phần số: "SP012" → 12
            if (int.TryParse(maCuoi.Substring(2), out int soHienTai))
                return $"SP{(soHienTai + 1):D3}"; // D3 = luôn 3 chữ số: 001, 002...

            return $"SP{Guid.NewGuid().ToString()[..6].ToUpper()}"; // fallback nếu parse lỗi
        }


        [HttpPost]
        public async Task<IActionResult> themSanPham([FromBody] ThemSanPhamDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var sp = new SanPham
            {
                MaSp = await sinhMaSanPham(),
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

            return CreatedAtAction(nameof(chiTietSanPham), new { maSP = sp.MaSp }, new {sp.MaSp, sp.TenSp, sp.GiaBan});
        }

        [HttpPut("{maSP}")]
        public async Task<IActionResult> suaSanPham(string maSP, [FromBody] SuaSanPhamDto dto)
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
