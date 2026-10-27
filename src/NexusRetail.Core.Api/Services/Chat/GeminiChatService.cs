
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace NexusRetail.Core.Api.Services.Chat
{
    public class GeminiChatService : IGeminiChatService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<GeminiChatService> _logger;

        private const string DefaultModel = "gemini-1.5-flash";

        public GeminiChatService(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            ILogger<GeminiChatService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _logger = logger;
        }

        public async Task<(string Reply, string Source, string? Note)>
            GenerateConsultationAsync(
                string systemPrompt,
                string userMessage)
        {
            // 1. Lấy API Key từ cấu hình hoặc biến môi trường.
            string apiKey = _config["Gemini:ApiKey"] ?? "";

            if (string.IsNullOrWhiteSpace(apiKey) ||
                apiKey.Contains("YOUR_GEMINI_API_KEY",
                    StringComparison.OrdinalIgnoreCase))
            {
                apiKey = Environment.GetEnvironmentVariable(
                    "GEMINI_API_KEY") ?? "";
            }

            if (string.IsNullOrWhiteSpace(apiKey) ||
                apiKey.Contains("YOUR_GEMINI_API_KEY",
                    StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning(
                    "[GeminiChatService] Chưa cấu hình Gemini API Key.");

                return (
                    GenerateSmartRecommendation(userMessage),
                    "local_smart_catalog",
                    "Chưa cấu hình Gemini API Key."
                );
            }

            // 2. Chọn model từ appsettings.json.
            string configuredModel = DefaultModel;

            configuredModel = configuredModel.Trim();

            // Model chính được thử trước, sau đó là các model chính thức từ Google
            var modelsToTry = new List<string>();

            if (!string.IsNullOrWhiteSpace(configuredModel))
                modelsToTry.Add(configuredModel);

            var officialModels = new[] { "gemini-1.5-flash", "gemini-2.0-flash", "gemini-1.5-pro", "gemini-2.0-flash-lite" };
            foreach (var m in officialModels)
            {
                if (!modelsToTry.Contains(m, StringComparer.OrdinalIgnoreCase))
                {
                    modelsToTry.Add(m);
                }
            }

            // 3. Tạo HTTP client.
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(30);

            string lastError = "Không nhận được phản hồi từ Gemini.";

            // 4. Thử lần lượt các model.
            foreach (string model in modelsToTry)
            {
                try
                {
                    var requestBody = new
                    {
                        systemInstruction = new
                        {
                            parts = new[]
                            {
                                new { text = systemPrompt }
                            }
                        },
                        contents = new[]
                        {
                            new
                            {
                                role = "user",
                                parts = new[]
                                {
                                    new { text = userMessage }
                                }
                            }
                        },
                        generationConfig = new
                        {
                            temperature = 0.7,
                            maxOutputTokens = 2048
                        }
                    };

                    string url =
                        "https://generativelanguage.googleapis.com/" +
                        $"v1beta/models/{Uri.EscapeDataString(model)}:" +
                        "generateContent";

                    string json = JsonSerializer.Serialize(requestBody);

                    using var request = new HttpRequestMessage(
                        HttpMethod.Post, url);

                    request.Headers.Add("x-goog-api-key", apiKey);

                    request.Content = new StringContent(
                        json,
                        Encoding.UTF8,
                        "application/json");

                    _logger.LogInformation(
                        "[GeminiChatService] Gọi Gemini model {Model}.",
                        model);

                    using var response = await client.SendAsync(request);

                    string responseJson =
                        await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        lastError =
                            $"HTTP {(int)response.StatusCode}: " +
                            GetSafeErrorMessage(responseJson);

                        _logger.LogWarning(
                            "[GeminiChatService] Model {Model} trả về " +
                            "HTTP {StatusCode}.",
                            model,
                            (int)response.StatusCode);

                        // Chỉ chuyển model khi có khả năng model hoặc
                        // cấu hình request không hợp lệ.
                        if (response.StatusCode == HttpStatusCode.NotFound ||
                            response.StatusCode == HttpStatusCode.BadRequest)
                        {
                            continue;
                        }

                        // Ví dụ: API Key sai, hết quota hoặc lỗi máy chủ.
                        // Thử model khác không chắc giải quyết được.
                        break;
                    }

                    using var document =
                        JsonDocument.Parse(responseJson);

                    string reply = ExtractResponseText(
                        document.RootElement);

                    if (string.IsNullOrWhiteSpace(reply))
                    {
                        lastError =
                            "Gemini trả về thành công nhưng không có nội dung.";

                        _logger.LogWarning(
                            "[GeminiChatService] Model {Model} không có text.",
                            model);

                        continue;
                    }

                    _logger.LogInformation(
                        "[GeminiChatService] Nhận phản hồi từ model {Model}.",
                        model);

                    return (
                        reply.Trim(),
                        "gemini",
                        $"Google Gemini API ({model})"
                    );
                }
                catch (OperationCanceledException ex)
                {
                    lastError = ex.Message;

                    _logger.LogWarning(
                        ex,
                        "[GeminiChatService] Yêu cầu model {Model} bị " +
                        "hủy hoặc hết thời gian chờ.",
                        model);

                    // Không tiếp tục thử model khi request đã timeout.
                    break;
                }
                catch (HttpRequestException ex)
                {
                    lastError = ex.Message;

                    _logger.LogError(
                        ex,
                        "[GeminiChatService] Lỗi kết nối model {Model}.",
                        model);

                    break;
                }
                catch (JsonException ex)
                {
                    lastError = ex.Message;

                    _logger.LogWarning(
                        ex,
                        "[GeminiChatService] JSON trả về không hợp lệ " +
                        "từ model {Model}.",
                        model);

                    continue;
                }
                catch (Exception ex)
                {
                    lastError = ex.Message;

                    _logger.LogError(
                        ex,
                        "[GeminiChatService] Lỗi khi gọi model {Model}.",
                        model);

                    break;
                }
            }

            // 5. Fallback nếu Gemini không hoạt động.
            _logger.LogWarning(
                "[GeminiChatService] Chuyển sang tư vấn nội bộ. " +
                "Lý do: {Error}",
                lastError);

            return (
                GenerateSmartRecommendation(userMessage),
                "local_smart_catalog",
                $"Gemini không khả dụng: {lastError}"
            );
        }

        private static string ExtractResponseText(JsonElement root)
        {
            if (!root.TryGetProperty("candidates", out var candidates) ||
                candidates.ValueKind != JsonValueKind.Array ||
                candidates.GetArrayLength() == 0)
            {
                return string.Empty;
            }

            var candidate = candidates[0];

            if (!candidate.TryGetProperty("content", out var content) ||
                !content.TryGetProperty("parts", out var parts) ||
                parts.ValueKind != JsonValueKind.Array)
            {
                return string.Empty;
            }

            var result = new StringBuilder();

            foreach (var part in parts.EnumerateArray())
            {
                if (part.TryGetProperty("text", out var textElement) &&
                    textElement.ValueKind == JsonValueKind.String)
                {
                    string? text = textElement.GetString();

                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        if (result.Length > 0)
                            result.AppendLine();

                        result.Append(text);
                    }
                }
            }

            return result.ToString();
        }

        private static string GetSafeErrorMessage(string responseJson)
        {
            try
            {
                using var document = JsonDocument.Parse(responseJson);

                if (document.RootElement.TryGetProperty(
                        "error", out var error) &&
                    error.TryGetProperty("message", out var message))
                {
                    return message.GetString() ?? "Lỗi API không xác định.";
                }
            }
            catch (JsonException)
            {
                // Không phân tích được JSON lỗi.
            }

            // Không ghi nguyên response vào log vì có thể chứa thông tin
            // kỹ thuật không cần thiết.
            return "Gemini API trả về lỗi.";
        }

        public string GenerateSmartRecommendation(string message)
        {
            string q = message.ToLowerInvariant();

            if (q.Contains("game") || q.Contains("chơi") || q.Contains("mượt") || q.Contains("fps"))
            {
                return "Để chiến game mượt mà không lo drop FPS, Duy Mobile khuyên bạn nên chọn POCO X6 Pro 5G (8.800.000 ₫ - cấu hình Dimensity 8300 cực đỉnh phân khúc), hoặc các siêu phẩm iPhone 16 Pro Max / Galaxy S24 Ultra với màn hình 120Hz và chip hàng đầu hiện nay!";
            }

            if (q.Contains("chụp ảnh") || q.Contains("camera") || q.Contains("quay phim") || q.Contains("sống ảo"))
            {
                return "Nếu bạn đam mê nhiếp ảnh và quay video chất lượng cao, Xiaomi 14 Ultra (Quad 50MP cảm biến Leica 1-inch) và Galaxy S24 Ultra (200MP zoom 100x) là hai 'ông vua' camera hiện nay. Ngoài ra, iPhone 16 Pro với nút Camera Control mới cũng cho trải nghiệm chụp ảnh rất chuyên nghiệp!";
            }

            if (q.Contains("dưới 20") || q.Contains("dưới 15") || q.Contains("giá rẻ") || q.Contains("tiết kiệm") || q.Contains("sinh viên"))
            {
                return "Trong tầm giá tiết kiệm từ 4 đến 15 triệu, Duy Mobile có sẵn nhiều lựa chọn cực tốt: Redmi 13 (3.960.000 ₫), Samsung Galaxy A55 5G (9.350.000 ₫) và iPhone 13 128GB (13.200.000 ₫). Tất cả đều bảo hành chính hãng và hỗ trợ trả góp 0% lãi suất!";
            }

            if (q.Contains("iphone") || q.Contains("apple") || q.Contains("ios"))
            {
                return "Duy Mobile hiện sẵn hàng đầy đủ các dòng iPhone chính hãng VN/A: iPhone 16 Pro Max 256GB (34.980.000 ₫), iPhone 16 128GB (22.880.000 ₫) và iPhone 13 128GB (13.200.000 ₫). Máy mới 100%, bảo hành 12 tháng chính hãng và tặng kèm gói dán cường lực!";
            }

            if (q.Contains("samsung") || q.Contains("galaxy"))
            {
                return "Về Samsung Galaxy, các dòng máy nổi bật nhất tại Duy Mobile là Galaxy S24 Ultra 256GB (26.400.000 ₫ - Galaxy AI đỉnh cao), Galaxy Z Fold6 màn hình gập (41.800.000 ₫) và Galaxy A55 5G (9.350.000 ₫). Đang có ưu đãi trợ giá thu cũ đổi mới lên đến 2.000.000 ₫!";
            }

            if (q.Contains("xiaomi") || q.Contains("redmi") || q.Contains("poco"))
            {
                return "Dòng Xiaomi tại Duy Mobile nổi tiếng với cấu hình cực mạnh và sạc siêu nhanh: Xiaomi 14 5G (17.600.000 ₫), Redmi Note 13 Pro (4.950.000 ₫) và POCO X6 Pro 5G (8.800.000 ₫), bảo hành chính hãng lên đến 18 tháng!";
            }

            if (q.Contains("oppo") || q.Contains("reno"))
            {
                return "Dòng OPPO tại Duy Mobile đang rất 'hot' với chuyên gia chân dung Reno12 Pro 5G (18.150.000 ₫) và Reno11 5G (8.800.000 ₫) cùng siêu phẩm gập Find N3 Flip (22.000.000 ₫) với camera Hasselblad cao cấp!";
            }

            return "Chào bạn! Duy Mobile là hệ thống bán lẻ smartphone chính hãng từ Apple, Samsung, Xiaomi, OPPO và Vivo. Bạn có thể cho mình biết nhu cầu chính (chơi game, chụp ảnh, văn phòng) hoặc tầm ngân sách dự kiến để mình tư vấn máy tối ưu nhất nhé!";
        }
    }
}
