using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusRetail.Core.Api.BUS;
using NexusRetail.Core.Api.Data;
using NexusRetail.Shared.DTOs.NhapHang;

namespace NexusRetail.Core.Api.Controllers;

[Authorize(Roles = "Admin,Staff")]
[Route("api/nhap-hang")]
[ApiController]
public class NhapHangController : ControllerBase
{
    private readonly INhapHangBUS _bus;

    [ActivatorUtilitiesConstructor]
    public NhapHangController(INhapHangBUS bus)
    {
        _bus = bus;
    }

    public NhapHangController(RetailDbContext db)
        : this(new NhapHangBUS(db))
    {
    }

    [HttpGet("next-ma-hdn")]
    public async Task<IActionResult> GetNextMaHdn()
    {
        var nextCode = await _bus.GetNextMaHdnAsync();
        return Ok(new { maHdn = nextCode });
    }

    [HttpGet]
    public async Task<IActionResult> layDanhSachHoaDonNhap()
    {
        var list = await _bus.LayDanhSachHoaDonNhapAsync();
        return Ok(list);
    }

    [HttpGet("nha-cung-cap")]
    public async Task<IActionResult> layDanhSachNhaCungCap()
    {
        var list = await _bus.LayDanhSachNhaCungCapAsync();
        return Ok(list);
    }

    [HttpGet("nhan-vien")]
    public async Task<IActionResult> layDanhSachNhanVien()
    {
        var list = await _bus.LayDanhSachNhanVienAsync();
        return Ok(list);
    }

    [HttpGet("{maHdn}")]
    public async Task<IActionResult> layChiTietHoaDonNhap(string maHdn)
    {
        var result = await _bus.LayChiTietHoaDonNhapAsync(maHdn);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> nhapHang([FromBody] HoaDonNhapDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _bus.TaoHoaDonNhapAsync(dto);
        if (!result.IsSuccess || result.Data == null)
        {
            return result.ToActionResult();
        }

        return CreatedAtAction(
            nameof(layChiTietHoaDonNhap),
            new { maHdn = result.Data.MaHdn },
            result.Data);
    }
}
