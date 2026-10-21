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

        public ChatController(RetailDbContext db, IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
            _config = config;
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

            string geminiReply = await CallGeminiAsync(systemPrompt, request.Message);

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
                items = matchedItems
            });
        }

        private async Task<string> CallGeminiAsync(string systemPrompt, string userMessage)
        {
            string apiKey = _config["Gemini:ApiKey"] ?? "";
            if (string.IsNullOrEmpty(apiKey))
                return FallbackReply();

            string model = _config["Gemini:Model"] ?? "gemini-1.5-flash";
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

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

            try
            {
                var client = _httpClientFactory.CreateClient();
                var json = JsonSerializer.Serialize(requestBody);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                    return FallbackReply();

                var responseJson = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseJson);

                string reply = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString() ?? FallbackReply();

                return reply.Trim();
            }
            catch
            {
                return FallbackReply();
            }
        }

        private static string FallbackReply() =>
            "Xin lỗi, hệ thống tư vấn AI đang tạm thời không khả dụng. Bạn vui lòng liên hệ nhân viên cửa hàng để được hỗ trợ trực tiếp nhé!";
    }
}
