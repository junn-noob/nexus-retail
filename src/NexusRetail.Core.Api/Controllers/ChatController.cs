using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusRetail.Core.Api.Data;
using NexusRetail.Shared.Entities;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace NexusRetail.Core.Api.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly RetailDbContext _db;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<ChatController> _logger;

        public ChatController(RetailDbContext db, IHttpClientFactory httpClientFactory, IConfiguration config, ILogger<ChatController> logger)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
            _config = config;
            _logger = logger;
        }

        public class ChatRequestDto
        {
            public string Message { get; set; } = "";
        }

        [HttpPost]
        public async Task<IActionResult> TuVan([FromBody] ChatRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request?.Message))
            {
                return Ok(new
                {
                    reply = "Xin chào! Mình là trợ lý AI của Duy Mobile. Bạn đang cần tìm điện thoại theo nhu cầu gì (chụp ảnh, chơi game, pin trâu, khoảng giá bao nhiêu) hoặc cần tra cứu đơn hàng bằng SĐT?",
                    items = Array.Empty<object>()
                });
            }

            string text = request.Message.Trim().ToLower();

            // 1. Tra cứu đơn hàng theo số điện thoại (xử lý cục bộ, không cần Gemini)
            var phoneMatch = Regex.Match(text, @"(03|05|07|08|09)\d{8}");
            if (phoneMatch.Success)
            {
                string sdt = phoneMatch.Value;
                var hoaDons = await _db.HoaDonBans
                    .AsNoTracking()
                    .Include(h => h.MaKhachNavigation)
                    .Include(h => h.ChiTietHdbs).ThenInclude(ct => ct.MaSpNavigation)
                    .Where(h => h.MaKhachNavigation.DienThoai == sdt)
                    .OrderByDescending(h => h.NgayBan)
                    .ToListAsync();

                if (hoaDons.Count == 0)
                {
                    return Ok(new
                    {
                        reply = $"Duy Mobile không tìm thấy đơn hàng nào gắn với số điện thoại {sdt}. Bạn vui lòng kiểm tra lại số hoặc liên hệ hotline để được hỗ trợ nhé!",
                        items = Array.Empty<object>()
                    });
                }

                var sb = new StringBuilder($"🔎 Thông tin đơn hàng của SĐT {sdt}:\n");
                foreach (var hd in hoaDons)
                {
                    sb.AppendLine($"• HĐ {hd.MaHdb} ({hd.NgayBan:dd/MM/yyyy}) - Tổng: {hd.TongTien:N0}đ");
                    foreach (var ct in hd.ChiTietHdbs)
                    {
                        var bhThang = ct.MaSpNavigation?.ThoiGianBaoHanh ?? 12;
                        var hanBh = hd.NgayBan.AddMonths(bhThang);
                        sb.AppendLine($"  - {ct.MaSpNavigation?.TenSp ?? ct.MaSp} (SL: {ct.SoLuong}) | BH đến: {hanBh:dd/MM/yyyy}");
                    }
                }

                return Ok(new
                {
                    reply = sb.ToString(),
                    items = Array.Empty<object>()
                });
            }

            // 2. Lấy danh sách sản phẩm sẵn hàng để làm ngữ cảnh cho Gemini
            var sanPhams = await _db.SanPhams
                .AsNoTracking()
                .Where(sp => sp.SoLuong > 0)
                .OrderByDescending(sp => sp.GiaBan)
                .Take(30)
                .Select(sp => new
                {
                    sp.MaSp,
                    sp.TenSp,
                    sp.GiaBan,
                    sp.SoLuong,
                    sp.GhiChu,
                    TenNhanHieu = sp.MaNhanHieuNavigation != null ? sp.MaNhanHieuNavigation.TenNhanHieu : ""
                })
                .ToListAsync();

            var catalogText = string.Join("\n", sanPhams.Select(sp =>
                $"- {sp.TenSp} ({sp.TenNhanHieu}): {sp.GiaBan:N0}đ, Tồn: {sp.SoLuong} máy" +
                (string.IsNullOrEmpty(sp.GhiChu) ? "" : $", Mô tả: {sp.GhiChu}")
            ));

            // 3. Xây dựng prompt gọi Gemini
            string systemPrompt = $"""
                Bạn là trợ lý AI tư vấn bán điện thoại của cửa hàng Duy Mobile.
                Nhiệm vụ của bạn là tư vấn khách hàng chọn điện thoại phù hợp dựa trên nhu cầu và ngân sách.
                Chỉ tư vấn các sản phẩm có trong danh sách sau (đây là hàng đang sẵn có):

                {catalogText}

                Quy tắc trả lời:
                - Trả lời bằng tiếng Việt, thân thiện, ngắn gọn (tối đa 3-5 câu).
                - Luôn đề xuất 1-3 sản phẩm cụ thể từ danh sách trên nếu phù hợp.
                - Nếu không có sản phẩm phù hợp, thông báo lịch sự và hỏi thêm nhu cầu.
                - Không bịa thêm sản phẩm ngoài danh sách.
                - Không trả lời các câu hỏi không liên quan đến tư vấn điện thoại hoặc cửa hàng.
                """;

            var (geminiReply, source, note) = await CallGeminiAsync(systemPrompt, request.Message, catalogText, sanPhams);

            // 4. Trích xuất tên sản phẩm được đề cập trong reply để tạo nút quick-view
            var matchedItems = sanPhams
                .Where(sp => geminiReply.Contains(sp.TenSp, StringComparison.OrdinalIgnoreCase))
                .Take(3)
                .Select(sp => new
                {
                    id = sp.MaSp,
                    name = sp.TenSp,
                    price = sp.GiaBan.ToString("N0") + " đ"
                })
                .ToList<object>();

            // 5. Ghi log tư vấn
            try
            {
                decimal? maxBudget = null;
                var budgetMatch = Regex.Match(text, @"(\d+)\s*(triệu|tr|m)");
                if (budgetMatch.Success && decimal.TryParse(budgetMatch.Groups[1].Value, out decimal millions))
                    maxBudget = millions * 1_000_000m;

                string? topMaSp = matchedItems.Count > 0
                    ? sanPhams.FirstOrDefault(sp => geminiReply.Contains(sp.TenSp, StringComparison.OrdinalIgnoreCase))?.MaSp
                    : null;

                _db.ChatbotTuVanLogs.Add(new ChatbotTuVanLog
                {
                    NhuCauKhach = request.Message,
                    NganSachToiDa = maxBudget,
                    MaSpGoiY = topMaSp,
                    PhanHoiChatbot = geminiReply,
                    ThoiGian = DateTime.Now
                });
                await _db.SaveChangesAsync();
            }
            catch { /* Bỏ qua lỗi ghi log, không ảnh hưởng trải nghiệm người dùng */ }

            return Ok(new
            {
                reply = geminiReply,
                items = matchedItems,
                source = source,
                note = note
            });
        }

        private async Task<(string reply, string source, string? note)> CallGeminiAsync(string systemPrompt, string userMessage, string catalogText, dynamic sanPhams)
        {
            string apiKey = _config["Gemini:ApiKey"] ?? "";
            if (string.IsNullOrEmpty(apiKey) || apiKey.Contains("YOUR_GEMINI_API_KEY"))
            {
                apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") ?? "";
            }

            if (string.IsNullOrWhiteSpace(apiKey) || apiKey.Contains("YOUR_GEMINI_API_KEY"))
            {
                _logger.LogInformation("[Chatbot] Chưa cấu hình Gemini:ApiKey trong appsettings.json. Hệ thống sử dụng bộ tư vấn thông minh từ danh mục kho hàng.");
                return (GenerateSmartRecommendation(userMessage), "local_smart_catalog", "Chưa cấu hình API Key (đang dùng placeholder YOUR_GEMINI_API_KEY_HERE)");
            }

            // Danh sách model Google Generative AI để thử (ưu tiên model cấu hình, gemini-3.8-flash, rồi fallback)
            var configuredModel = _config["Gemini:Model"] ?? "gemini-3.8-flash";
            var modelsToTry = new List<string>();
            if (!string.IsNullOrWhiteSpace(configuredModel)) modelsToTry.Add(configuredModel);
            if (!modelsToTry.Contains("gemini-3.8-flash")) modelsToTry.Add("gemini-3.8-flash");
            if (!modelsToTry.Contains("gemini-2.5-flash")) modelsToTry.Add("gemini-2.5-flash");
            if (!modelsToTry.Contains("gemini-2.0-flash")) modelsToTry.Add("gemini-2.0-flash");
            if (!modelsToTry.Contains("gemini-1.5-flash")) modelsToTry.Add("gemini-1.5-flash");

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
                    maxOutputTokens = 512
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
                    _logger.LogInformation("[Chatbot] Đang gửi yêu cầu đến Google Gemini API (model: {Model})...", model);
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
                                _logger.LogInformation("[Chatbot] Nhận phản hồi thành công từ Google Gemini (model: {Model})", model);
                                return (reply.Trim(), "gemini", $"Google Gemini API ({model})");
                            }
                        }
                    }
                    else
                    {
                        var errorBody = await response.Content.ReadAsStringAsync();
                        lastError = $"HTTP {(int)response.StatusCode}: {errorBody}";
                        _logger.LogWarning("[Chatbot] Google Gemini API trả lỗi ({Model}): {Error}", model, lastError);

                        // Tự động phân tích gợi ý model mới từ phản hồi lỗi của Google (ví dụ: models/gemini-3.8-flash)
                        var recMatch = Regex.Match(errorBody, @"models/(gemini-[a-zA-Z0-9.\-_]+)");
                        if (recMatch.Success)
                        {
                            string recommendedModel = recMatch.Groups[1].Value;
                            if (!modelsToTry.Contains(recommendedModel))
                            {
                                _logger.LogInformation("[Chatbot] Google khuyến nghị chuyển sang model: {RecModel}. Tự động gọi thử ngay...", recommendedModel);
                                modelsToTry.Insert(index, recommendedModel);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    lastError = ex.Message;
                    _logger.LogError(ex, "[Chatbot] Lỗi kết nối khi gọi Google Gemini API ({Model})", model);
                }
            }

            _logger.LogWarning("[Chatbot] Gọi Gemini thất bại sau khi thử tất cả model. Lỗi cuối: {Error}. Tự động fallback sang tư vấn nội bộ.", lastError);
            return (GenerateSmartRecommendation(userMessage), "local_smart_catalog", $"Gemini Error: {lastError}");
        }

        private string GenerateSmartRecommendation(string message)
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
