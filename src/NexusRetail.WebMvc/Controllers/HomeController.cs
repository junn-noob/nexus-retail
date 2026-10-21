using Microsoft.AspNetCore.Mvc;
using NexusRetail.Shared.DTOs.Common;
using NexusRetail.Shared.DTOs.SanPham;
using NexusRetail.WebMvc.Models;
using System.Diagnostics;

namespace NexusRetail.WebMvc.Controllers;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _factory;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IHttpClientFactory factory, ILogger<HomeController> logger)
    {
        _factory = factory;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int page = 1, string? q = null, string? price = null, string? brand = null)
    {
        var client = _factory.CreateClient("CoreApi");

        string url = $"api/san-pham?page={page}&pageSize=9";

        if (!string.IsNullOrWhiteSpace(brand))
            url += $"&brand={Uri.EscapeDataString(brand)}";

        if (!string.IsNullOrWhiteSpace(q))
            url += $"&q={Uri.EscapeDataString(q)}";

        if (!string.IsNullOrWhiteSpace(price))
        {
            url += price switch
            {
                "lt20" => "&maxPrice=20000000",
                "mid"  => "&minPrice=20000000&maxPrice=30000000",
                "gt30" => "&minPrice=30000000",
                _ => ""
            };
        }

        ViewBag.CurrentPrice = price;
        ViewBag.CurrentQ = q;
        ViewBag.CurrentBrand = brand;

        try
        {
            var data = await client.GetFromJsonAsync<PagedResultDto<SanPhamDto>>(url);
            return View(data ?? new PagedResultDto<SanPhamDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy danh sách sản phẩm từ CoreApi (url: {Url})", url);
            return View(new PagedResultDto<SanPhamDto>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> LaySanPhamTheoNhanHieu(string? maNhanHieu, int page = 1)
    {
        var client = _factory.CreateClient("CoreApi");

        try
        {
            string url = string.IsNullOrWhiteSpace(maNhanHieu)
                ? $"api/san-pham?page={page}&pageSize=9"
                : $"api/san-pham/nhan-hieu/{Uri.EscapeDataString(maNhanHieu)}?page={page}&pageSize=9";

            var data = await client.GetFromJsonAsync<PagedResultDto<SanPhamDto>>(url);

            return Json(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi lấy sản phẩm theo nhãn hiệu: {MaNhanHieu}", maNhanHieu);
            return Json(new PagedResultDto<SanPhamDto>());
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> TraCuu(string? sdt)
    {
        ViewBag.Sdt = sdt;
        if (string.IsNullOrWhiteSpace(sdt))
            return View();

        var client = _factory.CreateClient("CoreApi");
        try
        {
            var response = await client.GetAsync($"api/ban-hang/tra-cuu/{sdt.Trim()}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(content);
                return View(data);
            }
        }
        catch { /* CoreApi not reachable */ }

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
