using NexusRetail.Shared.DTOs.BanHang;
using NexusRetail.WebMvc.Models;
using System.Text.Json;

namespace NexusRetail.WebMvc.Services.Orders
{
    public class OrderApiClient : IOrderApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<OrderApiClient> _logger;

        public OrderApiClient(HttpClient client, ILogger<OrderApiClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<(bool Success, string Message, string? MaHdb, decimal TongTien)> CreateOrderAsync(HoaDonBanDto dto)
        {
            try
            {
                var response = await _client.PostAsJsonAsync("api/ban-hang/thanh-toan", dto);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<HoaDonBanResponseDto>();
                    return (true, "Đặt hàng thành công!", result?.MaHdb, result?.TongTien ?? 0m);
                }

                var errorMsg = "Đặt hàng không thành công, vui lòng thử lại!";
                try
                {
                    var errObj = await response.Content.ReadFromJsonAsync<JsonElement>();
                    if (errObj.TryGetProperty("message", out var m))
                    {
                        errorMsg = m.GetString() ?? errorMsg;
                    }
                }
                catch { }

                return (false, errorMsg, null, 0m);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[OrderApiClient] Lỗi khi gọi api/ban-hang/thanh-toan");
                return (false, "Không thể kết nối đến máy chủ thanh toán!", null, 0m);
            }
        }

        public async Task<JsonElement?> GetOrdersByPhoneAsync(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return null;

            try
            {
                var response = await _client.GetAsync($"api/ban-hang/tra-cuu/{Uri.EscapeDataString(phone.Trim())}");
                if (!response.IsSuccessStatusCode) return null;

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<JsonElement>(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[OrderApiClient] Lỗi tra cứu đơn hàng qua SĐT {Phone}", phone);
                return null;
            }
        }

        public async Task<(List<AdminOrderListItem> Items, int TotalPages, int TotalItems)> GetOrdersListAsync(int page = 1, int pageSize = 15, string? q = null)
        {
            var items = new List<AdminOrderListItem>();
            int totalPages = 1;
            int totalItems = 0;

            try
            {
                string url = $"api/ban-hang/danh-sach?page={page}&pageSize={pageSize}";
                if (!string.IsNullOrWhiteSpace(q))
                    url += $"&q={Uri.EscapeDataString(q.Trim())}";

                var response = await _client.GetFromJsonAsync<JsonElement>(url);

                if (response.TryGetProperty("items", out var itemsElem))
                {
                    foreach (var it in itemsElem.EnumerateArray())
                    {
                        items.Add(new AdminOrderListItem
                        {
                            MaHdb = it.TryGetProperty("maHdb", out var p1) ? p1.GetString() ?? "" : "",
                            NgayBan = it.TryGetProperty("ngayBan", out var p2) && p2.TryGetDateTime(out var dt) ? dt : DateTime.Now,
                            TongTien = it.TryGetProperty("tongTien", out var p3) && p3.TryGetDecimal(out var tt) ? tt : 0m,
                            TenKhach = it.TryGetProperty("tenKhach", out var p4) ? p4.GetString() ?? "" : "",
                            DienThoai = it.TryGetProperty("dienThoai", out var p5) ? p5.GetString() ?? "" : "",
                            TenNv = it.TryGetProperty("tenNv", out var p6) ? p6.GetString() ?? "" : "",
                            PhuongThucThanhToan = it.TryGetProperty("phuongThucThanhToan", out var p7) ? p7.GetString() ?? "Cash" : "Cash",
                            TrangThaiThanhToan = it.TryGetProperty("trangThaiThanhToan", out var p8) ? p8.GetString() ?? "DaThanhToan" : "DaThanhToan",
                            TrangThaiDonHang = it.TryGetProperty("trangThaiDonHang", out var p9) ? p9.GetString() ?? "HoanThanh" : "HoanThanh",
                            DiaChiGiaoHang = it.TryGetProperty("diaChiGiaoHang", out var p10) ? p10.GetString() ?? "" : ""
                        });
                    }
                }

                if (response.TryGetProperty("totalPages", out var tp)) totalPages = tp.GetInt32();
                if (response.TryGetProperty("totalItems", out var ti)) totalItems = ti.GetInt32();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[OrderApiClient] Lỗi lấy danh sách đơn hàng Admin");
            }

            return (items, totalPages, totalItems);
        }

        public async Task<AdminOrderDetailViewModel?> GetOrderDetailAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;

            try
            {
                var response = await _client.GetFromJsonAsync<JsonElement>($"api/ban-hang/chi-tiet/{Uri.EscapeDataString(id)}");
                var vm = new AdminOrderDetailViewModel
                {
                    MaHdb = response.TryGetProperty("maHdb", out var p1) ? p1.GetString() ?? "" : "",
                    NgayBan = response.TryGetProperty("ngayBan", out var p2) && p2.TryGetDateTime(out var dt) ? dt : DateTime.Now,
                    TongTien = response.TryGetProperty("tongTien", out var p3) && p3.TryGetDecimal(out var tt) ? tt : 0m,
                    TenKhach = response.TryGetProperty("tenKhach", out var p4) ? p4.GetString() ?? "" : "",
                    DienThoai = response.TryGetProperty("dienThoai", out var p5) ? p5.GetString() ?? "" : "",
                    TenNv = response.TryGetProperty("tenNv", out var p6) ? p6.GetString() ?? "" : "",
                    PhuongThucThanhToan = response.TryGetProperty("phuongThucThanhToan", out var p7) ? p7.GetString() ?? "Cash" : "Cash",
                    TrangThaiThanhToan = response.TryGetProperty("trangThaiThanhToan", out var p8) ? p8.GetString() ?? "DaThanhToan" : "DaThanhToan",
                    TrangThaiDonHang = response.TryGetProperty("trangThaiDonHang", out var p9) ? p9.GetString() ?? "HoanThanh" : "HoanThanh",
                    DiaChiGiaoHang = response.TryGetProperty("diaChiGiaoHang", out var p10) ? p10.GetString() ?? "" : "",
                    GhiChu = response.TryGetProperty("ghiChu", out var p11) ? p11.GetString() ?? "" : "",
                    SoTienDaThanhToan = response.TryGetProperty("soTienDaThanhToan", out var p12) && p12.TryGetDecimal(out var paid) ? paid : 0m,
                    MaGiaoDich = response.TryGetProperty("maGiaoDich", out var p13) ? p13.GetString() ?? "" : ""
                };

                if (response.TryGetProperty("chiTiet", out var ctList))
                {
                    foreach (var ct in ctList.EnumerateArray())
                    {
                        vm.ChiTiet.Add(new AdminOrderItem
                        {
                            MaSp = ct.GetProperty("maSp").GetString() ?? "",
                            TenSp = ct.GetProperty("tenSp").GetString() ?? "",
                            MaMau = ct.TryGetProperty("maMau", out var pMm) ? pMm.GetString() : null,
                            TenMau = ct.TryGetProperty("tenMau", out var pTm) ? pTm.GetString() : null,
                            MaHex = ct.TryGetProperty("maHex", out var pMh) ? pMh.GetString() : null,
                            SoLuong = ct.GetProperty("soLuong").GetInt32(),
                            DonGia = ct.GetProperty("donGia").GetDecimal(),
                            KhuyenMai = ct.GetProperty("khuyenMai").GetDecimal(),
                            ThanhTien = ct.GetProperty("thanhTien").GetDecimal()
                        });
                    }
                }

                return vm;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[OrderApiClient] Lỗi nạp chi tiết đơn hàng {Id}", id);
                return null;
            }
        }

        public async Task<(bool Success, string Message)> UpdateOrderStatusAsync(string maHdb, CapNhatTrangThaiDonDto dto)
        {
            if (string.IsNullOrWhiteSpace(maHdb))
                return (false, "Mã đơn hàng không hợp lệ.");

            try
            {
                var response = await _client.PatchAsJsonAsync($"api/ban-hang/{Uri.EscapeDataString(maHdb.Trim())}/trang-thai", dto);
                if (response.IsSuccessStatusCode)
                    return (true, "Cập nhật trạng thái đơn hàng thành công!");

                string errMsg = "Không thể cập nhật trạng thái đơn hàng.";
                try
                {
                    var json = await response.Content.ReadFromJsonAsync<JsonElement>();
                    if (json.TryGetProperty("message", out var msg))
                        errMsg = msg.GetString() ?? errMsg;
                }
                catch { }
                return (false, errMsg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[OrderApiClient] Lỗi cập nhật trạng thái đơn {MaHdb}", maHdb);
                return (false, "Lỗi kết nối máy chủ.");
            }
        }

        public async Task<(bool IsPaid, decimal PaidAmount, decimal ExpectedAmount, string Message, string? TransactionCode)> GetBankPaymentStatusAsync(string maHdb, decimal? expectedAmount = null)
        {
            if (string.IsNullOrWhiteSpace(maHdb))
                return (false, 0m, expectedAmount ?? 0m, "Mã đơn hàng không hợp lệ.", null);

            try
            {
                string url = $"api/webhook/bank/status/{Uri.EscapeDataString(maHdb.Trim())}";
                if (expectedAmount.HasValue && expectedAmount.Value > 0)
                    url += $"?expectedAmount={expectedAmount.Value}";

                var res = await _client.GetFromJsonAsync<JsonElement>(url);
                bool isPaid = res.TryGetProperty("isPaid", out var p1) && p1.GetBoolean();
                decimal paid = res.TryGetProperty("paidAmount", out var p2) && p2.TryGetDecimal(out var pa) ? pa : 0m;
                decimal exp = res.TryGetProperty("expectedAmount", out var p3) && p3.TryGetDecimal(out var ea) ? ea : (expectedAmount ?? 0m);
                string msg = res.TryGetProperty("message", out var p4) ? p4.GetString() ?? "Đang chờ thanh toán..." : "Đang chờ thanh toán...";
                string? txCode = res.TryGetProperty("transactionCode", out var p5) ? p5.GetString() : null;
                return (isPaid, paid, exp, msg, txCode);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[OrderApiClient] Lỗi kiểm tra thanh toán đơn {MaHdb}", maHdb);
                return (false, 0m, expectedAmount ?? 0m, "Đang chờ máy chủ đối soát...", null);
            }
        }

        public async Task<(bool Success, string Message)> SimulateBankPaymentAsync(string maHdb, decimal amount)
        {
            if (string.IsNullOrWhiteSpace(maHdb) || amount <= 0)
                return (false, "Thông tin giả lập thanh toán không hợp lệ.");

            try
            {
                var response = await _client.PostAsJsonAsync("api/webhook/bank/simulate", new { maHdb = maHdb.Trim(), amount });
                if (response.IsSuccessStatusCode)
                    return (true, $"Đã giả lập chuyển khoản thành công cho đơn {maHdb}!");

                return (false, $"Máy chủ từ chối giả lập (HTTP {(int)response.StatusCode}).");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[OrderApiClient] Lỗi giả lập thanh toán đơn {MaHdb}", maHdb);
                return (false, "Không thể kết nối máy chủ CoreApi.");
            }
        }
    }
}
