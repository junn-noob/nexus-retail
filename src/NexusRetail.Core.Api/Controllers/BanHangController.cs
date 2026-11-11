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

        // Nếu gọi qua HTTP từ khách vãng lai hoặc Customer (không phải Admin/Staff),
        // không cho phép tự gán MaNV thu ngân hay tự áp chiết khấu trên giá niêm yết.
        bool hasHttpContext = HttpContext != null;
        bool isStaffOrAdmin = hasHttpContext && (User.IsInRole("Admin") || User.IsInRole("Staff"));
        if (hasHttpContext && !isStaffOrAdmin)
        {
            dto.MaNv = null;
            if (dto.DanhSachMon != null)
            {
                foreach (var item in dto.DanhSachMon)
                {
                    item.KhuyenMai = 0m;
                }
            }
        }

        var result = await _bus.ThanhToanAsync(dto);
        return result.ToActionResult();
    }

    [AllowAnonymous]
    [HttpGet("tra-cuu/{sdt}")]
    public async Task<IActionResult> traCuuDonHang(string sdt)
    {
        bool isStaffOrAdmin = HttpContext != null && (User.IsInRole("Admin") || User.IsInRole("Staff"));
        var result = await _bus.TraCuuDonHangAsync(sdt, maskPii: !isStaffOrAdmin);
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
    [HttpPatch("{id}/trang-thai")]
    public async Task<IActionResult> capNhatTrangThai(string id, [FromBody] CapNhatTrangThaiDonDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _bus.CapNhatTrangThaiAsync(id, dto);
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
