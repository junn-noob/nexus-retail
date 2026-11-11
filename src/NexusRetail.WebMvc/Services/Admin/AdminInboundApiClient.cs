using System.Text.Json;
using NexusRetail.Shared.DTOs.NhapHang;
using NexusRetail.WebMvc.Models;

namespace NexusRetail.WebMvc.Services.Admin
{
    public class AdminInboundApiClient : IAdminInboundApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<AdminInboundApiClient> _logger;

        public AdminInboundApiClient(HttpClient client, ILogger<AdminInboundApiClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<List<AdminInboundInvoiceItem>> GetInboundInvoicesAsync()
        {
            try
            {
                return await _client.GetFromJsonAsync<List<AdminInboundInvoiceItem>>("api/nhap-hang") ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AdminInboundApiClient] Lỗi tải danh sách hóa đơn nhập");
                return new();
            }
        }

        public async Task<HoaDonNhapResponseDto?> GetInboundInvoiceDetailAsync(string maHdn)
        {
            if (string.IsNullOrWhiteSpace(maHdn)) return null;
            try
            {
                var response = await _client.GetAsync($"api/nhap-hang/{Uri.EscapeDataString(maHdn.Trim())}");
                if (!response.IsSuccessStatusCode) return null;
                return await response.Content.ReadFromJsonAsync<HoaDonNhapResponseDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AdminInboundApiClient] Lỗi tải chi tiết hóa đơn nhập {MaHdn}", maHdn);
                return null;
            }
        }

        public async Task<List<AdminSupplierOption>> GetSuppliersAsync()
        {
            try
            {
                return await _client.GetFromJsonAsync<List<AdminSupplierOption>>("api/nhap-hang/nha-cung-cap") ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AdminInboundApiClient] Lỗi tải danh sách nhà cung cấp");
                return new();
            }
        }

        public async Task<List<AdminEmployeeOption>> GetEmployeesAsync()
        {
            try
            {
                return await _client.GetFromJsonAsync<List<AdminEmployeeOption>>("api/nhap-hang/nhan-vien") ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AdminInboundApiClient] Lỗi tải danh sách nhân viên");
                return new();
            }
        }

        public async Task<string?> GetNextInboundCodeAsync()
        {
            try
            {
                var json = await _client.GetFromJsonAsync<JsonElement>("api/nhap-hang/next-ma-hdn");
                return json.TryGetProperty("maHdn", out var prop) ? prop.GetString() : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AdminInboundApiClient] Lỗi lấy mã HDN tiếp theo");
                return null;
            }
        }

        public async Task<(bool Success, string? MaHdn, string? ErrorMessage)> CreateInboundInvoiceAsync(HoaDonNhapDto dto)
        {
            try
            {
                var response = await _client.PostAsJsonAsync("api/nhap-hang", dto);
                if (response.IsSuccessStatusCode)
                {
                    var created = await response.Content.ReadFromJsonAsync<HoaDonNhapResponseDto>();
                    return (true, created?.MaHdn, null);
                }

                var msg = await AdminApiHelper.ReadErrorMessageAsync(response, "Không thể lập hóa đơn nhập.");
                return (false, null, msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AdminInboundApiClient] Lỗi tạo hóa đơn nhập");
                return (false, null, "Không thể kết nối đến máy chủ");
            }
        }
    }
}

