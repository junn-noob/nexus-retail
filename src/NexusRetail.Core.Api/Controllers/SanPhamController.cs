using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusRetail.Core.Api.BUS;
using NexusRetail.Core.Api.Data;
using NexusRetail.Shared.DTOs.SanPham;

namespace NexusRetail.Core.Api.Controllers;

[Route("api/san-pham")]
[ApiController]
public class SanPhamController : ControllerBase
{
    private readonly ISanPhamBUS _bus;

    [ActivatorUtilitiesConstructor]
    public SanPhamController(ISanPhamBUS bus)
    {
        _bus = bus;
    }

    public SanPhamController(RetailDbContext db, IAdoNetDataProvider? adoNet = null)
        : this(new SanPhamBUS(db, adoNet))
    {
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> layDanhSach(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 9,
        [FromQuery] string? q = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] string? brand = null)
    {
        var result = await _bus.LayDanhSachAsync(page, pageSize, q, minPrice, maxPrice, brand);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{maSP}")]
    public async Task<IActionResult> chiTietSanPham(string maSP)
    {
        var sanPham = await _bus.ChiTietSanPhamAsync(maSP);
        return sanPham == null ? NotFound() : Ok(sanPham);
    }

    [AllowAnonymous]
    [HttpGet("{maSP}/mau-sac")]
    public async Task<IActionResult> layMauSacSanPham(string maSP)
    {
        var list = await _bus.LayDanhSachMauSacTheoSpAsync(maSP);
        return Ok(list);
    }

    [AllowAnonymous]
    [HttpGet("mau-sac")]
    public async Task<IActionResult> layTatCaMauSac()
    {
        var list = await _bus.LayTatCaMauSacAsync();
        return Ok(list);
    }

    [AllowAnonymous]
    [HttpGet("nhan-hieu/{maNhanHieu}")]
    public async Task<IActionResult> laySanPhamTheoNhanHieu(
        string maNhanHieu,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 9)
    {
        var result = await _bus.LayTheoNhanHieuAsync(maNhanHieu, page, pageSize);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("bo-loc-danh-muc")]
    public async Task<IActionResult> layBoLocDanhMuc()
    {
        var filters = await _bus.LayBoLocDanhMucAsync();
        return Ok(filters);
    }

    [AllowAnonymous]
    [HttpGet("tim-kiem")]
    public async Task<IActionResult> timKiem(
        [FromQuery] string? keyword,
        [FromQuery] string? tenLoai,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice)
    {
        var danhSach = await _bus.TimKiemAsync(keyword, tenLoai, minPrice, maxPrice);
        return Ok(danhSach);
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpPost]
    public async Task<IActionResult> themSanPham([FromBody] ThemSanPhamDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _bus.ThemSanPhamAsync(dto);
        if (!result.IsSuccess || result.Data == null)
        {
            return result.ToActionResult();
        }

        return CreatedAtAction(
            nameof(chiTietSanPham),
            new { maSP = result.Data.MaSp },
            new { result.Data.MaSp, result.Data.TenSp, result.Data.GiaBan });
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpPut("{maSP}")]
    public async Task<IActionResult> suaSanPham(string maSP, [FromBody] SuaSanPhamDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _bus.SuaSanPhamAsync(maSP, dto);
        return result.ToActionResult();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{maSP}")]
    public async Task<IActionResult> xoaSanPham(string maSP)
    {
        var result = await _bus.XoaSanPhamAsync(maSP);
        return result.ToActionResult();
    }
}
