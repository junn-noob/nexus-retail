using Microsoft.AspNetCore.Mvc;
using NexusRetail.Shared.DTOs.Common;
using NexusRetail.Shared.DTOs.SanPham;
using NexusRetail.WebMvc.Models;
using NexusRetail.WebMvc.Services.Catalog;
using NexusRetail.WebMvc.Services.Orders;
using System.Diagnostics;

namespace NexusRetail.WebMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICatalogApiClient _catalogClient;
        private readonly IOrderApiClient _orderClient;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ICatalogApiClient catalogClient, IOrderApiClient orderClient, ILogger<HomeController> logger)
        {
            _catalogClient = catalogClient;
            _orderClient = orderClient;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1, string? q = null, string? price = null, string? brand = null)
        {
            ViewBag.CurrentPrice = price;
            ViewBag.CurrentQ = q;
            ViewBag.CurrentBrand = brand;

            var data = await _catalogClient.GetProductsAsync(page, 9, q, brand, price);
            return View(data);
        }

        [HttpGet]
        public async Task<IActionResult> LaySanPhamTheoNhanHieu(string? maNhanHieu, int page = 1)
        {
            var data = await _catalogClient.GetProductsByBrandAsync(maNhanHieu, page, 9);
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

            var data = await _orderClient.GetOrdersByPhoneAsync(sdt);
            return View(data);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
