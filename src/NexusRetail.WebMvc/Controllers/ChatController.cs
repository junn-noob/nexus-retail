using Microsoft.AspNetCore.Mvc;

namespace NexusRetail.WebMvc.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly IHttpClientFactory _factory;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IHttpClientFactory factory, ILogger<ChatController> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public class ChatRequest
        {
            public string Message { get; set; } = "";
        }

        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] ChatRequest req)
        {
            try
            {
                var client = _factory.CreateClient("CoreApi");
                var response = await client.PostAsJsonAsync("api/chat", req);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Content(content, "application/json");
                }
                else
                {
                    _logger.LogWarning("CoreApi chat endpoint returned status {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không thể kết nối đến CoreApi tại https://localhost:7143/");
            }

            // Fallback phản hồi thân thiện nếu CoreApi chưa khởi động hoặc đang bận
            return Ok(new
            {
                reply = "Chào bạn! Duy Mobile hiện đang có sẵn đầy đủ các siêu phẩm iPhone 15 Pro Max, Samsung Galaxy S24 Ultra, Xiaomi 14 và OPPO Reno11 chính hãng VNA. Bạn có thể cho mình biết nhu cầu hoặc ngân sách để mình hỗ trợ nhé!",
                items = new[]
                {
                    new { id = "SP01", name = "iPhone 15 Pro Max 256GB", price = "28.600.000 ₫" },
                    new { id = "SP02", name = "Samsung Galaxy S24 Ultra 256GB", price = "26.400.000 ₫" }
                },
                source = "mvc_fallback",
                note = "CoreApi chưa khởi động hoặc đang bận"
            });
        }
    }
}
