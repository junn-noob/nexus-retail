using Microsoft.AspNetCore.Mvc;
using NexusRetail.Shared.Entities;

namespace NexusRetail.WebMvc.ViewComponents
{
    public class NhanHieuViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _factory;

        public NhanHieuViewComponent(IHttpClientFactory factory) { this._factory = factory; }

        public async Task<IViewComponentResult> InvokeAsync() 
        {
            var client = _factory.CreateClient("CoreApi");

            try
            {
                var data = await client.GetFromJsonAsync<List<NhanHieu>>("api/nhan-hieu");
                return View(data ?? new List<NhanHieu>());
            }
            catch
            {
                var fallback = new List<NhanHieu>
                {
                    new NhanHieu { MaNhanHieu = "NH01", TenNhanHieu = "Apple" },
                    new NhanHieu { MaNhanHieu = "NH02", TenNhanHieu = "Samsung" },
                    new NhanHieu { MaNhanHieu = "NH03", TenNhanHieu = "Xiaomi" },
                    new NhanHieu { MaNhanHieu = "NH04", TenNhanHieu = "OPPO" },
                    new NhanHieu { MaNhanHieu = "NH05", TenNhanHieu = "Realme" }
                };
                return View(fallback);
            }
        }
    }
}
