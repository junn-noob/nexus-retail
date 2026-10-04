using Microsoft.AspNetCore.Mvc;
using NexusRetail.Shared.DTOs.Common;
using NexusRetail.Shared.DTOs.SanPham;
using System.Net.Http.Json;

namespace NexusRetail.WebMvc.Controllers;

public class ProductsController : Controller
{
    private readonly IHttpClientFactory _factory;

    public ProductsController(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    [HttpGet]
    public async Task<IActionResult> Quick(string id)
    {
        var client = _factory.CreateClient("CoreApi");

        var response = await client.GetAsync(
            $"api/san-pham/{id}"
        );

        if (!response.IsSuccessStatusCode)
            return NotFound();

        var data = await response.Content
            .ReadFromJsonAsync<SanPhamDto>();

        if (data == null)
            return NotFound();

        return PartialView("_Modal", data);
    }


    // Lấy danh sách SP cho dropdown so sánh
    [HttpGet]
    public async Task<IActionResult> CompareProducts()
    {
        var client = _factory.CreateClient("CoreApi");

        var response = await client.GetAsync(
            "api/san-pham?page=1&pageSize=100"
        );

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode);

        var data = await response.Content
            .ReadFromJsonAsync<PagedResultDto<SanPhamDto>>();

        return Json(data);
    }


    // So sánh 2 sản phẩm
    [HttpGet]
    public async Task<IActionResult> Compare(
        string id,
        string otherId)
    {
        var client = _factory.CreateClient("CoreApi");

        var responseA = await client.GetAsync(
            $"api/san-pham/{id}"
        );

        var responseB = await client.GetAsync(
            $"api/san-pham/{otherId}"
        );

        if (!responseA.IsSuccessStatusCode ||
            !responseB.IsSuccessStatusCode)
        {
            return NotFound();
        }

        var a = await responseA.Content
            .ReadFromJsonAsync<SanPhamDto>();

        var b = await responseB.Content
            .ReadFromJsonAsync<SanPhamDto>();

        if (a == null || b == null)
            return NotFound();

        var data = new List<SanPhamDto>
        {
            a,
            b
        };

        return PartialView("_Compare", data);
    }
}