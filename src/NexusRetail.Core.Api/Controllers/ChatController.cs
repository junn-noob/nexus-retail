using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusRetail.Core.Api.Data;
using NexusRetail.Core.Api.Services.Chat;
using NexusRetail.Shared.Entities;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace NexusRetail.Core.Api.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly RetailDbContext _db;
        private readonly IGeminiChatService _geminiService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(
            RetailDbContext db,
            IGeminiChatService geminiService,
            ILogger<ChatController> logger)
        {
            _db = db;
            _geminiService = geminiService;
            _logger = logger;
        }

        public class ChatRequestDto
        {
            public string Message { get; set; } = "";
        }

        // POST: api/chat
        [HttpPost]
        public async Task<IActionResult> TuVan([FromBody] ChatRequestDto? request)
        {
            if (string.IsNullOrWhiteSpace(request?.Message))
            {
                return Ok(new
                {
                    reply = "Xin chào! Mình là trợ lý AI của Duy Mobile. Bạn đang tìm điện thoại theo ngân sách hay nhu cầu nào (chơi game, chụp ảnh, pin trâu, iPhone, Samsung...)? Bạn cũng có thể gửi số điện thoại để tra cứu đơn hàng nhé!",
                    items = Array.Empty<object>(),
                    source = "local",
                    note = (string?)null
                });
            }

            string originalMessage = request.Message.Trim();
            string text = originalMessage.ToLowerInvariant();

            // =========================================================
            // 1. TRA CỨU ĐƠN HÀNG THEO SỐ ĐIỆN THOẠI
            // =========================================================
            var phoneMatch = Regex.Match(text, @"(?<!\d)(?:\+?84|0)(?:3|5|7|8|9)\d{8}(?!\d)");
            if (phoneMatch.Success)
            {
                string sdt = phoneMatch.Value;
                if (sdt.StartsWith("+84")) sdt = "0" + sdt.Substring(3);
                else if (sdt.StartsWith("84")) sdt = "0" + sdt.Substring(2);

                var hoaDons = await _db.HoaDonBans
                    .AsNoTracking()
                    .Include(h => h.MaKhachNavigation)
                    .Include(h => h.ChiTietHdbs)
                        .ThenInclude(ct => ct.MaSpNavigation)
                    .Where(h => h.MaKhachNavigation != null && h.MaKhachNavigation.DienThoai == sdt)
                    .OrderByDescending(h => h.NgayBan)
                    .ToListAsync();

                if (hoaDons.Count == 0)
                {
                    return Ok(new
                    {
                        reply = $"Duy Mobile không tìm thấy đơn hàng nào gắn với số điện thoại {sdt}. Bạn vui lòng kiểm tra lại số điện thoại hoặc gọi tổng đài 1800.6868 để được nhân viên hỗ trợ ngay nhé!",
                        items = Array.Empty<object>(),
                        source = "local",
                        note = (string?)null
                    });
                }

                var sb = new StringBuilder();
                sb.AppendLine($"📦 THÔNG TIN ĐƠN HÀNG CỦA SĐT {sdt}:");
                foreach (var hd in hoaDons)
                {
                    sb.AppendLine($"\n• Mã HĐ: {hd.MaHdb} | Ngày lập: {hd.NgayBan:dd/MM/yyyy HH:mm} | Tổng tiền: {hd.TongTien:N0} ₫");
                    foreach (var ct in hd.ChiTietHdbs)
                    {
                        var thoiGianBh = ct.MaSpNavigation?.ThoiGianBaoHanh ?? 12;
                        var hanBaoHanh = hd.NgayBan.AddMonths(thoiGianBh);
                        sb.AppendLine($"  - {ct.MaSpNavigation?.TenSp ?? ct.MaSp} (SL: {ct.SoLuong}) | Hạn BH: {hanBaoHanh:dd/MM/yyyy}");
                    }
                }
                sb.AppendLine("\nQuý khách khi cần bảo hành chỉ cần mang máy đến cửa hàng Duy Mobile gần nhất kèm SĐT trên là được phục vụ chu đáo!");

                return Ok(new
                {
                    reply = sb.ToString(),
                    items = Array.Empty<object>(),
                    source = "local",
                    note = (string?)null
                });
            }

            // =========================================================
            // 2. PHÂN TÍCH Ý ĐỊNH, THƯƠNG HIỆU & NGÂN SÁCH CỦA KHÁCH
            // =========================================================
            decimal? maxBudget = ExtractBudget(text);

            string? detectedBrand = null;
            if (text.Contains("iphone") || text.Contains("apple") || text.Contains("ios")) detectedBrand = "Apple";
            else if (text.Contains("samsung") || text.Contains("galaxy")) detectedBrand = "Samsung";
            else if (text.Contains("xiaomi") || text.Contains("redmi") || text.Contains("poco")) detectedBrand = "Xiaomi";
            else if (text.Contains("oppo") || text.Contains("reno") || text.Contains("find")) detectedBrand = "OPPO";
            else if (text.Contains("vivo")) detectedBrand = "Vivo";

            bool isGaming = text.Contains("game") || text.Contains("chơi") || text.Contains("mượt") || text.Contains("fps") || text.Contains("cấu hình") || text.Contains("hiệu năng") || text.Contains("chip") || text.Contains("snapdragon");
            bool isCamera = text.Contains("chụp ảnh") || text.Contains("camera") || text.Contains("quay phim") || text.Contains("sống ảo") || text.Contains("chân dung") || text.Contains("zoom") || text.Contains("selfie") || text.Contains("nhiếp ảnh");
            bool isBattery = text.Contains("pin") || text.Contains("pin trâu") || text.Contains("pin lâu") || text.Contains("sạc nhanh");
            bool isFold = text.Contains("gập") || text.Contains("fold") || text.Contains("flip");
            bool isCheap = text.Contains("giá rẻ") || text.Contains("tiết kiệm") || text.Contains("học sinh") || text.Contains("sinh viên") || text.Contains("bình dân") || text.Contains("dưới 5") || text.Contains("dưới 4");

            // =========================================================
            // 3. TRUY VẤN VÀ CHẤM ĐIỂM SẢN PHẨM PHÙ HỢP TỪ DATABASE
            // =========================================================
            var inStockProducts = await _db.SanPhams
                .AsNoTracking()
                .Include(sp => sp.MaNhanHieuNavigation)
                .Include(sp => sp.MaLoaiNavigation)
                .Where(sp => sp.SoLuong > 0)
                .ToListAsync();

            var scoredList = inStockProducts.Select(p =>
            {
                int score = 0;
                string brand = p.MaNhanHieuNavigation?.TenNhanHieu ?? "";
                string nameLower = p.TenSp.ToLowerInvariant();
                string noteLower = (p.GhiChu ?? "").ToLowerInvariant();

                // Điểm theo Thương hiệu
                if (!string.IsNullOrEmpty(detectedBrand))
                {
                    if (brand.Equals(detectedBrand, StringComparison.OrdinalIgnoreCase) || nameLower.Contains(detectedBrand.ToLowerInvariant()))
                    {
                        score += 200;
                    }
                    else
                    {
                        score -= 200; // Khách hỏi hãng này thì loại trừ hãng khác
                    }
                }

                // Điểm theo Ngân sách
                if (maxBudget.HasValue)
                {
                    if (p.GiaBan <= maxBudget.Value)
                    {
                        score += 100;
                        decimal ratio = p.GiaBan / maxBudget.Value;
                        score += (int)(ratio * 30); // Càng sát ngân sách càng được ưu tiên
                    }
                    else
                    {
                        score -= 250; // Vượt ngân sách
                    }
                }

                // Điểm theo Nhu cầu Gaming
                if (isGaming)
                {
                    if (noteLower.Contains("snapdragon 8") || noteLower.Contains("a17 pro") || noteLower.Contains("a18") ||
                        noteLower.Contains("dimensity 8300") || noteLower.Contains("dimensity 9200") ||
                        nameLower.Contains("poco") || nameLower.Contains("pro max") || nameLower.Contains("ultra"))
                    {
                        score += 50;
                    }
                }

                // Điểm theo Nhu cầu Camera
                if (isCamera)
                {
                    if (noteLower.Contains("200mp") || noteLower.Contains("leica") || noteLower.Contains("hasselblad") ||
                        noteLower.Contains("camera control") || noteLower.Contains("tele 5x") ||
                        nameLower.Contains("ultra") || nameLower.Contains("pro max") || nameLower.Contains("x100"))
                    {
                        score += 50;
                    }
                }

                // Điểm Màn hình gập
                if (isFold)
                {
                    if (nameLower.Contains("fold") || nameLower.Contains("flip"))
                    {
                        score += 150;
                    }
                }

                // Điểm Pin trâu / Bình dân
                if (isBattery && (noteLower.Contains("5000mah") || noteLower.Contains("pin trâu") || nameLower.Contains("plus")))
                {
                    score += 30;
                }

                if (isCheap && p.GiaBan < 7000000m)
                {
                    score += 40;
                }

                return new { Product = p, Score = score };
            })
            .Where(x => x.Score >= 0)
            .OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.Product.GiaBan)
            .Select(x => x.Product)
            .ToList();

            // Nếu không tìm thấy máy theo đúng điều kiện (ví dụ: iPhone dưới 5 triệu không có)
            List<SanPham> relevantProducts;
            if (scoredList.Count > 0)
            {
                relevantProducts = scoredList;
            }
            else
            {
                // Fallback: Tìm sản phẩm gần nhất
                if (!string.IsNullOrEmpty(detectedBrand))
                {
                    relevantProducts = inStockProducts
                        .Where(p => (p.MaNhanHieuNavigation?.TenNhanHieu ?? "").Equals(detectedBrand, StringComparison.OrdinalIgnoreCase))
                        .OrderBy(p => p.GiaBan)
                        .ToList();
                }
                else if (maxBudget.HasValue)
                {
                    relevantProducts = inStockProducts
                        .Where(p => p.GiaBan <= maxBudget.Value)
                        .OrderByDescending(p => p.GiaBan)
                        .ToList();
                }
                else
                {
                    relevantProducts = inStockProducts.OrderByDescending(p => p.GiaBan).Take(15).ToList();
                }
            }

            // =========================================================
            // 4. XÂY DỰNG DANH MỤC CONTEXT & SYSTEM PROMPT CHO AI
            // =========================================================
            var topCandidateProducts = relevantProducts.Take(8).ToList();
            string catalogText = string.Join("\n", topCandidateProducts.Select(p =>
                $"- Mã SP: {p.MaSp} | Tên: {p.TenSp} | Hãng: {p.MaNhanHieuNavigation?.TenNhanHieu ?? ""} | Giá bán: {p.GiaBan:N0} ₫ | Tồn kho: {p.SoLuong} | Đặc điểm: {p.GhiChu ?? "Chính hãng VN/A"}"
            ));

            string systemPrompt = $"""
                Bạn là trợ lý AI tư vấn bán hàng chuyên nghiệp, nhiệt tình của Duy Mobile (Cửa hàng di động chính hãng).

                DANH SÁCH SẢN PHẨM PHÙ HỢP TRONG KHO DUY MOBILE:
                {catalogText}

                NGUYÊN TẮC TƯ VẤN:
                1. Trả lời bằng tiếng Việt lịch sự, thân thiện, súc tích (khoảng 3 - 5 câu).
                2. ƯU TIÊN giới thiệu cụ thể 1 - 3 mẫu máy trong danh sách trên, ghi rõ Tên sản phẩm và Giá bán niêm yết chính xác (ví dụ: iPhone 16 Pro Max 256GB - 34.980.000 ₫).
                3. Giải thích ngắn gọn lý do tại sao mẫu máy đó phù hợp với nhu cầu/ngân sách của khách (chip mạnh, camera nét, pin trâu, bảo hành chính hãng...).
                4. Nhắc khách có thể nhấn vào các thẻ sản phẩm bên dưới để xem chi tiết và đặt hàng.
                5. Cam kết chính hãng 100%, bảo hành 12-18 tháng, hỗ trợ trả góp 0% và thu cũ đổi mới trợ giá cao.
                """;

            // =========================================================
            // 5. GỌI GEMINI HOẶC DÙNG SMART CATALOG FALLBACK
            // =========================================================
            var (geminiReply, source, note) = await _geminiService.GenerateConsultationAsync(systemPrompt, originalMessage);

            // =========================================================
            // 6. KHỚP SẢN PHẨM TRẢ VỀ FRONTEND (MATCHED CARDS)
            // =========================================================
            var matchedItems = new List<object>();
            var selectedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Ưu tiên các sản phẩm được nhắc đến trong câu trả lời
            foreach (var sp in topCandidateProducts)
            {
                string baseName = ExtractBaseName(sp.TenSp);
                bool isMentioned = geminiReply.Contains(sp.TenSp, StringComparison.OrdinalIgnoreCase) ||
                                  geminiReply.Contains(baseName, StringComparison.OrdinalIgnoreCase) ||
                                  geminiReply.Contains(sp.MaSp, StringComparison.OrdinalIgnoreCase);

                if (isMentioned && selectedIds.Add(sp.MaSp))
                {
                    matchedItems.Add(new
                    {
                        id = sp.MaSp,
                        name = sp.TenSp,
                        price = sp.GiaBan.ToString("N0") + " ₫"
                    });

                    if (matchedItems.Count >= 3) break;
                }
            }

            // Nếu câu trả lời AI khái quát không nhắc đủ 3 máy, bù thêm từ danh sách xếp hạng cao nhất
            if (matchedItems.Count < 3 && topCandidateProducts.Count > 0)
            {
                foreach (var sp in topCandidateProducts)
                {
                    if (selectedIds.Add(sp.MaSp))
                    {
                        matchedItems.Add(new
                        {
                            id = sp.MaSp,
                            name = sp.TenSp,
                            price = sp.GiaBan.ToString("N0") + " ₫"
                        });

                        if (matchedItems.Count >= 3) break;
                    }
                }
            }

            // =========================================================
            // 7. GHI LOG TƯ VẤN VÀO CƠ SỞ DỮ LIỆU
            // =========================================================
            try
            {
                string? topMaSp = topCandidateProducts.FirstOrDefault()?.MaSp;
                _db.ChatbotTuVanLogs.Add(new ChatbotTuVanLog
                {
                    NhuCauKhach = originalMessage,
                    NganSachToiDa = maxBudget,
                    MaSpGoiY = topMaSp,
                    PhanHoiChatbot = geminiReply,
                    ThoiGian = DateTime.Now
                });
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[ChatController] Lỗi ghi log tư vấn");
            }

            return Ok(new
            {
                reply = geminiReply,
                items = matchedItems,
                source = source,
                note = note
            });
        }

        // Tách bỏ hậu tố dung lượng (256GB, 128GB, v.v.) để so sánh thông minh
        private static string ExtractBaseName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "";
            var cleaned = Regex.Replace(fullName, @"\s+(\d+GB(\/\d+GB)?|\d+TB|5G|4G)\b", "", RegexOptions.IgnoreCase);
            return cleaned.Trim();
        }

        // Tạo câu trả lời tư vấn nội bộ sống động theo kho hàng thật khi chưa có API Key
        private static string GenerateLocalConsultationReply(
            string userMessage,
            string? brand,
            decimal? maxBudget,
            List<SanPham> candidates)
        {
            if (candidates.Count == 0)
            {
                return "Chào bạn! Hiện tại các sản phẩm khớp với tiêu chí bạn tìm đang tạm hết hàng tại quầy. Bạn có thể cho mình biết thêm tầm ngân sách hoặc dòng máy khác để Duy Mobile tư vấn chi tiết hơn nhé!";
            }

            var sb = new StringBuilder();
            sb.Append("Chào bạn! ");

            if (!string.IsNullOrEmpty(brand))
            {
                sb.Append($"Với nhu cầu tìm điện thoại **{brand}**");
                if (maxBudget.HasValue) sb.Append($" trong tầm giá dưới **{maxBudget.Value:N0} ₫**");
                sb.AppendLine(", Duy Mobile đặc biệt gợi ý các siêu phẩm chính hãng đang bán rất chạy tại cửa hàng:");
            }
            else if (maxBudget.HasValue)
            {
                sb.AppendLine($"Với ngân sách tầm **{maxBudget.Value:N0} ₫**, Duy Mobile gợi ý cho bạn những mẫu smartphone đáng mua nhất hiện nay:");
            }
            else
            {
                sb.AppendLine("Dựa trên nhu cầu của bạn, Duy Mobile xin gợi ý các dòng smartphone chính hãng nổi bật hàng đầu:");
            }

            foreach (var sp in candidates.Take(3))
            {
                string note = !string.IsNullOrWhiteSpace(sp.GhiChu) ? $" ({sp.GhiChu})" : "";
                sb.AppendLine($"• **{sp.TenSp}** - Giá: **{sp.GiaBan:N0} ₫**{note}");
            }

            sb.Append("Tất cả máy đều mới 100% chính hãng, sẵn hàng bảo hành 12-18 tháng và hỗ trợ trả góp 0%. Bạn nhấn vào các thẻ sản phẩm bên dưới để xem chi tiết nhé!");

            return sb.ToString();
        }

        // Phân tích ngân sách theo triệu đồng (ví dụ: "dưới 15 triệu" => 15.000.000)
        private static decimal? ExtractBudget(string text)
        {
            var match = Regex.Match(
                text,
                @"(?:dưới|tối đa|không quá|khoảng|tầm|ngân sách)?\s*(\d+(?:[.,]\d+)?)\s*(triệu|tr|m)\b",
                RegexOptions.IgnoreCase);

            if (!match.Success) return null;

            string numberText = match.Groups[1].Value.Replace(',', '.');
            if (!decimal.TryParse(numberText, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal millions))
            {
                return null;
            }

            if (millions <= 0) return null;
            return millions * 1_000_000m;
        }
    }
}
