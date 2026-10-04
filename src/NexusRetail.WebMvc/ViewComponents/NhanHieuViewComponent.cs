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

            var data = await client.GetFromJsonAsync<List<NhanHieu>>("api/NhanHieu");

            return View(data);
        }
    }
}
