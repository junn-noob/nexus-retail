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

        public async Task<(List<AdminOrderListItem> Items, int TotalPages, int TotalItems)> GetOrdersListAsync(int page = 1, int pageSize = 15)
        {
            var items = new List<AdminOrderListItem>();
            int totalPages = 1;
            int totalItems = 0;

            try
            {
                var response = await _client.GetFromJsonAsync<JsonElement>($"api/ban-hang/danh-sach?page={page}&pageSize={pageSize}");

                if (response.TryGetProperty("items", out var itemsElem))
                {
                    foreach (var it in itemsElem.EnumerateArray())
                    {
                        items.Add(new AdminOrderListItem
                        {
                            MaHdb = it.GetProperty("maHdb").GetString() ?? "",
                            NgayBan = it.GetProperty("ngayBan").GetDateTime(),
                            TongTien = it.GetProperty("tongTien").GetDecimal(),
                            TenKhach = it.GetProperty("tenKhach").GetString() ?? "",
                            DienThoai = it.GetProperty("dienThoai").GetString() ?? "",
                            TenNv = it.GetProperty("tenNv").GetString() ?? ""
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
                    MaHdb = response.GetProperty("maHdb").GetString() ?? "",
                    NgayBan = response.GetProperty("ngayBan").GetDateTime(),
                    TongTien = response.GetProperty("tongTien").GetDecimal(),
                    TenKhach = response.GetProperty("tenKhach").GetString() ?? "",
                    DienThoai = response.GetProperty("dienThoai").GetString() ?? "",
                    TenNv = response.GetProperty("tenNv").GetString() ?? ""
                };

                if (response.TryGetProperty("chiTiet", out var ctList))
                {
                    foreach (var ct in ctList.EnumerateArray())
                    {
                        vm.ChiTiet.Add(new AdminOrderItem
                        {
                            MaSp = ct.GetProperty("maSp").GetString() ?? "",
                            TenSp = ct.GetProperty("tenSp").GetString() ?? "",
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
    }
}
