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

    public async Task<IActionResult> Index(int page = 1)
    {
        var client = _factory.CreateClient("CoreApi");

        var data = await client.GetFromJsonAsync<PagedResultDto<SanPhamDto>>($"api/san-pham?page={page}&pageSize=6");

        return View(data);
    }

    [HttpGet]
    public async Task<IActionResult> LaySanPhamTheoNhanHieu(string maNhanHieu, int page = 1)
    {
        var client = _factory.CreateClient("CoreApi");

        var data = await client.GetFromJsonAsync<PagedResultDto<SanPhamDto>>(
            $"api/san-pham/nhan-hieu/{maNhanHieu}?page={page}&pageSize=2");

        return Json(data);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
