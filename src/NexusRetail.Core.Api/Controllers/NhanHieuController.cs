using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusRetail.Core.Api.BUS;
using NexusRetail.Core.Api.Data;
using NexusRetail.Shared.Entities;

namespace NexusRetail.Core.Api.Controllers;

public class NhanHieuUpsertDto
{
    [Required(ErrorMessage = "Tên nhãn hiệu không được để trống!")]
    [StringLength(100, ErrorMessage = "Tên nhãn hiệu tối đa 100 ký tự")]
    public string TenNhanHieu { get; set; } = string.Empty;
}

[Route("api/nhan-hieu")]
[ApiController]
public class NhanHieuController : ControllerBase
{
    private readonly RetailDbContext _db;

    public NhanHieuController(RetailDbContext db)
    {
        _db = db;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> layTatCa()
    {
        var nhanHieu = await _db.NhanHieus
            .AsNoTracking()
            .OrderBy(x => x.MaNhanHieu)
            .Select(x => new { x.MaNhanHieu, x.TenNhanHieu })
            .ToListAsync();
        return Ok(nhanHieu);
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpPost]
    public async Task<IActionResult> themNhanHieu([FromBody] NhanHieuUpsertDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var ten = dto.TenNhanHieu.Trim();
        if (await _db.NhanHieus.AnyAsync(x => x.TenNhanHieu.ToLower() == ten.ToLower()))
        {
            return BadRequest(new { message = $"Nhãn hiệu '{ten}' đã tồn tại!" });
        }

        var existingCodes = await _db.NhanHieus.Select(x => x.MaNhanHieu).ToListAsync();
        var maNh = CodeSequenceHelper.GenerateNextCode(existingCodes, "NH");

        var entity = new NhanHieu
        {
            MaNhanHieu = maNh,
            TenNhanHieu = ten
        };

        _db.NhanHieus.Add(entity);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(layTatCa), new { id = entity.MaNhanHieu }, new { entity.MaNhanHieu, entity.TenNhanHieu });
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpPut("{maNhanHieu}")]
    public async Task<IActionResult> suaNhanHieu(string maNhanHieu, [FromBody] NhanHieuUpsertDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var entity = await _db.NhanHieus.FirstOrDefaultAsync(x => x.MaNhanHieu == maNhanHieu);
        if (entity == null) return NotFound();

        entity.TenNhanHieu = dto.TenNhanHieu.Trim();
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{maNhanHieu}")]
    public async Task<IActionResult> xoaNhanHieu(string maNhanHieu)
    {
        var entity = await _db.NhanHieus.FirstOrDefaultAsync(x => x.MaNhanHieu == maNhanHieu);
        if (entity == null) return NotFound();

        if (await _db.SanPhams.AnyAsync(x => x.MaNhanHieu == maNhanHieu))
        {
            return BadRequest(new { message = "Không thể xóa nhãn hiệu đang có sản phẩm liên kết!" });
        }

        _db.NhanHieus.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
