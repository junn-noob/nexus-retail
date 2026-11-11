using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusRetail.Core.Api.BUS;
using NexusRetail.Core.Api.Data;
using NexusRetail.Core.Api.Services.Export;

namespace NexusRetail.Core.Api.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/bao-cao")]
[ApiController]
public class BaoCaoController : ControllerBase
{
    private const string ExcelMimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    private readonly IBaoCaoBUS _bus;

    [ActivatorUtilitiesConstructor]
    public BaoCaoController(IBaoCaoBUS bus)
    {
        _bus = bus;
    }

    public BaoCaoController(RetailDbContext db, IExcelExportService excelService, IAdoNetDataProvider? adoNet = null)
        : this(new BaoCaoBUS(db, excelService, adoNet))
    {
    }

    [HttpGet("sp-ban-chay")]
    public async Task<IActionResult> laySpBanChay(
        [FromQuery] int? month = null,
        [FromQuery] int? thang = null,
        [FromQuery] int? year = null,
        [FromQuery] int? nam = null)
    {
        int m = month ?? thang ?? 9;
        int y = year ?? nam ?? 2026;

        if (m < 1 || m > 12 || y < 2000)
            return BadRequest(new { message = "Tháng (1-12) hoặc năm không hợp lệ!" });

        return Ok(await _bus.LaySpBanChayAsync(m, y));
    }

    [HttpGet("sp-ban-chay/excel")]
    public async Task<IActionResult> xuatExcelSpBanChay(
        [FromQuery] int? month = null,
        [FromQuery] int? thang = null,
        [FromQuery] int? year = null,
        [FromQuery] int? nam = null)
    {
        int m = month ?? thang ?? 9;
        int y = year ?? nam ?? 2026;

        var fileBytes = await _bus.XuatExcelSpBanChayAsync(m, y);
        return File(fileBytes, ExcelMimeType, $"BaoCao_Top5_SanPham_Thang_{m}_{y}.xlsx");
    }

    [HttpGet("hdn-lon-nhat")]
    [HttpGet("nhap-lon-nhat-quy")]
    public async Task<IActionResult> layHdnLonNhat(
        [FromQuery] int? quarter = null,
        [FromQuery] int? quy = null,
        [FromQuery] int? year = null,
        [FromQuery] int? nam = null)
    {
        int q = quarter ?? quy ?? 3;
        int y = year ?? nam ?? 2026;

        if (q < 1 || q > 4 || y < 2000)
            return BadRequest(new { message = "Quý (1-4) hoặc năm không hợp lệ!" });

        return Ok(await _bus.LayHdnLonNhatAsync(q, y));
    }

    [HttpGet("hdn-lon-nhat/excel")]
    [HttpGet("nhap-lon-nhat-quy/excel")]
    public async Task<IActionResult> xuatExcelHdnLonNhat(
        [FromQuery] int? quarter = null,
        [FromQuery] int? quy = null,
        [FromQuery] int? year = null,
        [FromQuery] int? nam = null)
    {
        int q = quarter ?? quy ?? 3;
        int y = year ?? nam ?? 2026;

        var fileBytes = await _bus.XuatExcelHdnLonNhatAsync(q, y);
        return File(fileBytes, ExcelMimeType, $"BaoCao_HDN_LonNhat_Quy_{q}_{y}.xlsx");
    }

    [HttpGet("doanh-so-nam")]
    public async Task<IActionResult> layDoanhSoNam([FromQuery] int? year = null, [FromQuery] int? nam = null)
    {
        int y = year ?? nam ?? 2026;
        if (y < 2000)
            return BadRequest(new { message = "Năm không hợp lệ!" });

        return Ok(await _bus.LayDoanhSoNamAsync(y));
    }

    [HttpGet("doanh-so-nam/excel")]
    public async Task<IActionResult> xuatExcelDoanhSoNam([FromQuery] int? year = null, [FromQuery] int? nam = null)
    {
        int y = year ?? nam ?? 2026;
        var fileBytes = await _bus.XuatExcelDoanhSoNamAsync(y);
        return File(fileBytes, ExcelMimeType, $"BaoCao_DoanhSo_Nam_{y}.xlsx");
    }

    [HttpGet("nv-doanh-so-thap")]
    [HttpGet("nv-ban-it-nhat")]
    public async Task<IActionResult> layNvDoanhSoThap(
        [FromQuery] int? month = null,
        [FromQuery] int? thang = null,
        [FromQuery] int? year = null,
        [FromQuery] int? nam = null)
    {
        int m = month ?? thang ?? 9;
        int y = year ?? nam ?? 2026;

        if (m < 1 || m > 12 || y < 2000)
            return BadRequest(new { message = "Tháng hoặc năm không hợp lệ!" });

        return Ok(await _bus.LayNvDoanhSoThapAsync(m, y));
    }

    [HttpGet("nv-doanh-so-thap/excel")]
    [HttpGet("nv-ban-it-nhat/excel")]
    public async Task<IActionResult> xuatExcelNvDoanhSoThap(
        [FromQuery] int? month = null,
        [FromQuery] int? thang = null,
        [FromQuery] int? year = null,
        [FromQuery] int? nam = null)
    {
        int m = month ?? thang ?? 9;
        int y = year ?? nam ?? 2026;

        var fileBytes = await _bus.XuatExcelNvDoanhSoThapAsync(m, y);
        return File(fileBytes, ExcelMimeType, $"BaoCao_5NV_DoanhSoThap_Thang_{m}_{y}.xlsx");
    }

    [HttpGet("ton-kho")]
    [HttpGet("xuat-nhap-ton")]
    public async Task<IActionResult> layBaoCaoTonKho(
        [FromQuery] int? month = null,
        [FromQuery] int? thang = null,
        [FromQuery] int? year = null,
        [FromQuery] int? nam = null)
    {
        int m = month ?? thang ?? DateTime.Now.Month;
        int y = year ?? nam ?? DateTime.Now.Year;

        return Ok(await _bus.LayBaoCaoTonKhoAsync(m, y));
    }

    [HttpGet("ton-kho/excel")]
    [HttpGet("xuat-nhap-ton/excel")]
    public async Task<IActionResult> xuatExcelBaoCaoTonKho(
        [FromQuery] int? month = null,
        [FromQuery] int? thang = null,
        [FromQuery] int? year = null,
        [FromQuery] int? nam = null)
    {
        int m = month ?? thang ?? DateTime.Now.Month;
        int y = year ?? nam ?? DateTime.Now.Year;

        var fileBytes = await _bus.XuatExcelBaoCaoTonKhoAsync(m, y);
        return File(fileBytes, ExcelMimeType, $"BaoCao_XuatNhapTon_Thang_{m}_{y}.xlsx");
    }

    /// <summary>
    /// Dữ liệu thực tế cho biểu đồ Dashboard (doanh thu 6 tháng gần nhất & tỷ trọng hãng).
    /// </summary>
    [HttpGet("dashboard-charts")]
    public async Task<IActionResult> layDuLieuBieuDoDashboard()
    {
        return Ok(await _bus.LayDuLieuBieuDoDashboardAsync());
    }

    /// <summary>
    /// Truy vấn cảnh báo tồn kho thấp trực tiếp bằng câu lệnh SQL Text trên ADO.NET.
    /// </summary>
    [HttpGet("canh-bao-ton-kho-adonet")]
    public async Task<IActionResult> layCanhBaoTonKhoAdoNet([FromQuery] int nguong = 5)
    {
        if (nguong < 0)
            return BadRequest(new { message = "Ngưỡng tồn kho phải lớn hơn hoặc bằng 0." });

        return Ok(await _bus.LayCanhBaoTonKhoAdoNetAsync(nguong));
    }
}
