using Microsoft.AspNetCore.Mvc;
using NexusRetail.Shared.DTOs.SanPham;
using NexusRetail.WebMvc.Models;
using System.Diagnostics;
using X.PagedList.Extensions;

namespace NexusRetail.WebMvc.Controllers;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _factory;

    public HomeController(IHttpClientFactory factory) { this._factory = factory; }

    public async Task<IActionResult> Index(int? page)
    {
        var client = _factory.CreateClient("CoreApi");

        var data = await client.GetFromJsonAsync<List<SanPhamDtos>>("api/SanPhamAPI");

        int pageSize = 9;
        int pageNumber = page == null || page < 1 ? 1 : page.Value;
        var pageList = data.ToPagedList(pageNumber, pageSize);

        return View(pageList);
    }

    [HttpGet]
    public async Task<IActionResult> LaySanPhamTheoNhanHieu(string maNhanHieu)
    {
        var client = _factory.CreateClient("CoreApi");

        var data = await client.GetFromJsonAsync<List<SanPhamDtos>>(
            $"api/SanPhamAPI/NhanHieu/{maNhanHieu}");

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
