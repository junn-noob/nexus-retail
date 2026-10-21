using Microsoft.AspNetCore.Mvc;

namespace NexusRetail.WebMvc.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IHttpClientFactory _factory;

        public ChatController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public class ChatRequest
        {
            public string Message { get; set; } = "";
        }

        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] ChatRequest req)
        {
            var client = _factory.CreateClient("CoreApi");
            var response = await client.PostAsJsonAsync("api/chat", req);

            if (!response.IsSuccessStatusCode)
            {
                return Ok(new
                {
                    reply = "Hệ thống tư vấn đang bận. Bạn vui lòng thử lại sau giây lát nhé!",
                    items = Array.Empty<object>()
                });
            }

            var content = await response.Content.ReadAsStringAsync();
            return Content(content, "application/json");
        }
    }
}
