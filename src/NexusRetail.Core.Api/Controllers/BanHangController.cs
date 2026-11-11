using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusRetail.Core.Api.BUS;
using NexusRetail.Core.Api.Data;
using NexusRetail.Shared.DTOs.BanHang;

namespace NexusRetail.Core.Api.Controllers;

[Route("api/ban-hang")]
[ApiController]
public class BanHangController : ControllerBase
{
    private readonly IBanHangBUS _bus;

    [ActivatorUtilitiesConstructor]
    public BanHangController(IBanHangBUS bus)
    {
        _bus = bus;
    }

    public BanHangController(RetailDbContext db)
        : this(new BanHangBUS(db))
    {
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpGet("next-ma-hdb")]
    public async Task<IActionResult> GetNextMaHdb()
    {
        var nextCode = await _bus.GetNextMaHdbAsync();
        return Ok(new { maHdb = nextCode });
    }

    [AllowAnonymous]
    [HttpPost("thanh-toan")]
    public async Task<IActionResult> thanhToan([FromBody] HoaDonBanDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _bus.ThanhToanAsync(dto);
        return result.ToActionResult();
    }

    [AllowAnonymous]
    [HttpGet("tra-cuu/{sdt}")]
    public async Task<IActionResult> traCuuDonHang(string sdt)
    {
        var result = await _bus.TraCuuDonHangAsync(sdt);
        return result.ToActionResult();
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpGet("danh-sach")]
    public async Task<IActionResult> layDanhSach(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 15,
        [FromQuery] string? q = null)
    {
        var result = await _bus.LayDanhSachAsync(page, pageSize, q);
        return Ok(result);
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpGet("chi-tiet/{id}")]
    public async Task<IActionResult> layChiTiet(string id)
    {
        var result = await _bus.LayChiTietAsync(id);
        return result.ToActionResult();
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpGet("thong-ke-dashboard")]
    public async Task<IActionResult> layThongKeDashboard()
    {
        var stats = await _bus.LayThongKeDashboardAsync();
        return Ok(stats);
    }
}
