using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace NexusRetail.Core.Api.Services.Chat
{
    public class GeminiChatService : IGeminiChatService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<GeminiChatService> _logger;

        public GeminiChatService(IHttpClientFactory httpClientFactory, IConfiguration config, ILogger<GeminiChatService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _logger = logger;
        }

        public async Task<(string Reply, string Source, string? Note)> GenerateConsultationAsync(string systemPrompt, string userMessage)
        {
            string apiKey = _config["Gemini:ApiKey"] ?? "";
            if (string.IsNullOrEmpty(apiKey) || apiKey.Contains("YOUR_GEMINI_API_KEY"))
            {
                apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? "";
            }

            if (string.IsNullOrWhiteSpace(apiKey) || apiKey.Contains("YOUR_GEMINI_API_KEY"))
            {
                _logger.LogInformation("[GeminiChatService] Chưa cấu hình Gemini:ApiKey. Sử dụng bộ tư vấn thông minh từ danh mục kho hàng.");
                return (GenerateSmartRecommendation(userMessage), "local_smart_catalog", "Chưa cấu hình API Key");
            }

            var configuredModel = _config["Gemini:Model"] ?? "gemini-flash-lite-latest";
            var modelsToTry = new List<string>();
            if (!string.IsNullOrWhiteSpace(configuredModel)) modelsToTry.Add(configuredModel);
            if (!modelsToTry.Contains("gemini-flash-lite-latest")) modelsToTry.Add("gemini-flash-lite-latest");
            if (!modelsToTry.Contains("gemini-3.5-flash-lite")) modelsToTry.Add("gemini-3.5-flash-lite");
            if (!modelsToTry.Contains("gemini-3.8-flash")) modelsToTry.Add("gemini-3.8-flash");
            if (!modelsToTry.Contains("gemini-flash-latest")) modelsToTry.Add("gemini-flash-latest");

            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(15);

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[]
                        {
                            new { text = systemPrompt + "\n\nKhách hàng hỏi: " + userMessage }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.7,
                    maxOutputTokens = 2048
                }
            };

            string lastError = "";
            int index = 0;
            while (index < modelsToTry.Count)
            {
                var model = modelsToTry[index++];
                string url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";
                try
                {
                    _logger.LogInformation("[GeminiChatService] Đang gửi yêu cầu đến Google Gemini API (model: {Model})...", model);
                    var json = JsonSerializer.Serialize(requestBody);
                    using var content = new StringContent(json, Encoding.UTF8, "application/json");
                    using var response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseJson = await response.Content.ReadAsStringAsync();
                        using var doc = JsonDocument.Parse(responseJson);

                        if (doc.RootElement.TryGetProperty("candidates", out var candidates) &&
                            candidates.GetArrayLength() > 0 &&
                            candidates[0].TryGetProperty("content", out var candContent) &&
                            candContent.TryGetProperty("parts", out var parts) &&
                            parts.GetArrayLength() > 0 &&
                            parts[0].TryGetProperty("text", out var textEl))
                        {
                            string? reply = textEl.GetString();
                            if (!string.IsNullOrWhiteSpace(reply))
                            {
                                _logger.LogInformation("[GeminiChatService] Nhận phản hồi thành công từ Google Gemini (model: {Model})", model);
                                return (reply.Trim(), "gemini", $"Google Gemini API ({model})");
                            }
                        }
                    }
                    else
                    {
                        var errorBody = await response.Content.ReadAsStringAsync();
                        lastError = $"HTTP {(int)response.StatusCode}: {errorBody}";
                        _logger.LogWarning("[GeminiChatService] Google Gemini API trả lỗi ({Model}): {Error}", model, lastError);

                        var recMatches = Regex.Matches(errorBody, @"models/(gemini-[a-zA-Z0-9.\-_]+)");
                        foreach (Match recMatch in recMatches)
                        {
                            string candidateModel = recMatch.Groups[1].Value;
                            if (!string.Equals(candidateModel, model, StringComparison.OrdinalIgnoreCase) && !modelsToTry.Contains(candidateModel))
                            {
                                _logger.LogInformation("[GeminiChatService] Tìm thấy model thay thế: {RecModel}. Thử ngay...", candidateModel);
                                modelsToTry.Insert(index, candidateModel);
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    lastError = ex.Message;
                    _logger.LogError(ex, "[GeminiChatService] Lỗi kết nối khi gọi Gemini API ({Model})", model);
                }
            }

            _logger.LogWarning("[GeminiChatService] Fallback sang danh mục kho hàng do lỗi: {Error}", lastError);
            return (GenerateSmartRecommendation(userMessage), "local_smart_catalog", $"Gemini Error: {lastError}");
        }

        public string GenerateSmartRecommendation(string message)
        {
            string q = message.ToLower();

            if (q.Contains("game") || q.Contains("chơi") || q.Contains("mượt") || q.Contains("fps"))
            {
                return "Để chơi game mượt mà, cấu hình mạnh và tản nhiệt tốt, Duy Mobile đặc biệt khuyên bạn nên chọn iPhone 16 Pro Max hoặc Samsung Galaxy S24 Ultra. Cả 2 dòng máy đều trang bị chip xử lý đỉnh cao, màn hình 120Hz siêu nhạy và pin trâu cho trải nghiệm gaming trọn vẹn nhất!";
            }

            if (q.Contains("chụp ảnh") || q.Contains("camera") || q.Contains("quay phim") || q.Contains("sống ảo"))
            {
                return "Nếu bạn ưu tiên chụp ảnh sắc nét, màu sắc chân thực và chống rung quang học đỉnh cao, hãy tham khảo iPhone 16 Pro hoặc Samsung Galaxy S24 Ultra với ống kính tele tiềm vọng zoom xa ấn tượng và cảm biến độ phân giải cực lớn!";
            }

            if (q.Contains("dưới 20") || q.Contains("dưới 15") || q.Contains("giá rẻ") || q.Contains("tiết kiệm"))
            {
                return "Trong tầm giá tiết kiệm dưới 20 triệu, Duy Mobile có các lựa chọn cực kỳ đáng giá như iPhone 13, Xiaomi 14T Pro hoặc Samsung Galaxy S23 FE. Các sản phẩm đều được bảo hành chính hãng 12 tháng và hỗ trợ trả góp 0% lãi suất!";
            }

            if (q.Contains("iphone") || q.Contains("apple"))
            {
                return "Duy Mobile hiện là đại lý ủy quyền với đầy đủ các dòng iPhone chính hãng VNA từ iPhone 13, iPhone 15 đến iPhone 16 Series mới nhất, cam kết 1 đổi 1 trong 30 ngày và tặng kèm gói bảo hành rơi vỡ!";
            }

            if (q.Contains("samsung") || q.Contains("galaxy"))
            {
                return "Các dòng Samsung Galaxy tại Duy Mobile đang có chương trình trợ giá thu cũ đổi mới lên tới 3.000.000₫. Nổi bật nhất là Galaxy S24 Ultra và Galaxy Z Fold6 với các tính năng Galaxy AI thông minh vượt trội!";
            }

            return "Chào bạn! Duy Mobile có sẵn đầy đủ các mẫu smartphone cao cấp từ Apple, Samsung, Xiaomi với mức giá tốt nhất thị trường. Bạn có thể cho mình biết rõ hơn về ngân sách dự kiến hoặc nhu cầu chính (chơi game, chụp ảnh, làm việc) để mình tư vấn máy tối ưu nhất nhé!";
        }
    }
}
