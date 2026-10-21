using Microsoft.AspNetCore.Mvc;
using NexusRetail.WebMvc.Services.Catalog;

namespace NexusRetail.WebMvc.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ICatalogApiClient _catalogClient;

        public ProductsController(ICatalogApiClient catalogClient)
        {
            _catalogClient = catalogClient;
        }

        [HttpGet]
        [Route("Products/Quick/{id?}")]
        public async Task<IActionResult> Quick(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var data = await _catalogClient.GetProductByIdAsync(id);
            if (data == null) return NotFound();

            return PartialView("_Modal", data);
        }

        // Lấy danh sách SP cho dropdown so sánh
        [HttpGet]
        public async Task<IActionResult> CompareProducts()
        {
            var data = await _catalogClient.GetCompareProductsAsync();
            return Json(data);
        }

        // So sánh 2 sản phẩm
        [HttpGet]
        public async Task<IActionResult> Compare(string id, string otherId)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(otherId))
                return NotFound();

            var taskA = _catalogClient.GetProductByIdAsync(id);
            var taskB = _catalogClient.GetProductByIdAsync(otherId);

            await Task.WhenAll(taskA, taskB);

            var a = await taskA;
            var b = await taskB;

            if (a == null || b == null)
                return NotFound();

            return PartialView("_Compare", (a, b));
        }
    }
}