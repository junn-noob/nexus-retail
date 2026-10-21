using System.Text.Json;

namespace NexusRetail.WebMvc.Services.Chat
{
    public class ChatApiClient : IChatApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<ChatApiClient> _logger;

        public ChatApiClient(HttpClient client, ILogger<ChatApiClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<string> SendMessageJsonAsync(string message)
        {
            try
            {
                var response = await _client.PostAsJsonAsync("api/chat", new { message });
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }

                _logger.LogWarning("[ChatApiClient] CoreApi chat trả status {StatusCode}", response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ChatApiClient] Không thể kết nối đến CoreApi chat");
            }

            // Fallback phản hồi thân thiện nếu CoreApi gặp sự cố
            var fallback = new
            {
                reply = "Chào bạn! Duy Mobile hiện đang có sẵn đầy đủ các siêu phẩm iPhone 16 Pro Max, iPhone 15 Pro Max, Samsung Galaxy S24 Ultra, Xiaomi 14 và OPPO Reno11 chính hãng VNA. Bạn có thể cho mình biết nhu cầu hoặc ngân sách để mình hỗ trợ nhé!",
                items = new[]
                {
                    new { id = "SP01", name = "iPhone 16 Pro Max 256GB", price = "33.990.000 ₫" },
                    new { id = "SP02", name = "Samsung Galaxy S24 Ultra 256GB", price = "27.490.000 ₫" }
                },
                source = "mvc_fallback",
                note = "CoreApi chưa khởi động hoặc đang bận"
            };

            return JsonSerializer.Serialize(fallback);
        }
    }
}
