using Microsoft.AspNetCore.Mvc;
using NexusRetail.Shared.DTOs.Common;
using NexusRetail.Shared.DTOs.SanPham;
using NexusRetail.WebMvc.Models;
using System.Diagnostics;

namespace NexusRetail.WebMvc.Controllers;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _factory;

    public HomeController(IHttpClientFactory factory) { this._factory = factory; }

    public async Task<IActionResult> Index(int page = 1, string? q = null, string? price = null)
    {
        var client = _factory.CreateClient("CoreApi");

        var url = $"api/san-pham?page={page}&pageSize=9";
        if (!string.IsNullOrWhiteSpace(q))
            url += $"&q={Uri.EscapeDataString(q)}";
        if (!string.IsNullOrWhiteSpace(price))
            url += $"&price={price}";

        var data = await client.GetFromJsonAsync<PagedResultDto<SanPhamDto>>(url)
                   ?? new PagedResultDto<SanPhamDto>();

        return View(data);
    }

    [HttpGet]
    public async Task<IActionResult> LaySanPhamTheoNhanHieu(string maNhanHieu, int page = 1)
    {
        var client = _factory.CreateClient("CoreApi");

        var data = await client.GetFromJsonAsync<PagedResultDto<SanPhamDto>>(
            $"api/san-pham/nhan-hieu/{maNhanHieu}?page={page}&pageSize=9");

        return Json(data);
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
        catch { }

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
