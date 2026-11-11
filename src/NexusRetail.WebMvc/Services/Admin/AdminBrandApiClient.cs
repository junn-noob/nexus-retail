using NexusRetail.WebMvc.Models;

namespace NexusRetail.WebMvc.Services.Admin
{
    public class AdminBrandApiClient : IAdminBrandApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<AdminBrandApiClient> _logger;

        public AdminBrandApiClient(HttpClient client, ILogger<AdminBrandApiClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<List<BrandOption>> GetBrandsAsync()
        {
            try
            {
                return await _client.GetFromJsonAsync<List<BrandOption>>("api/nhan-hieu") ?? new();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AdminBrandApiClient] Lỗi tải danh sách nhãn hiệu");
                return new();
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateBrandAsync(string tenNhanHieu)
        {
            try
            {
                var response = await _client.PostAsJsonAsync("api/nhan-hieu", new { TenNhanHieu = tenNhanHieu });
                if (response.IsSuccessStatusCode) return (true, null);
                return (false, await AdminApiHelper.ReadErrorMessageAsync(response, "Không thể thêm nhãn hiệu."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AdminBrandApiClient] Lỗi thêm nhãn hiệu");
                return (false, "Không thể kết nối đến máy chủ");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateBrandAsync(string maNhanHieu, string tenNhanHieu)
        {
            try
            {
                var response = await _client.PutAsJsonAsync($"api/nhan-hieu/{Uri.EscapeDataString(maNhanHieu)}", new { TenNhanHieu = tenNhanHieu });
                if (response.IsSuccessStatusCode) return (true, null);
                return (false, await AdminApiHelper.ReadErrorMessageAsync(response, "Không thể cập nhật nhãn hiệu."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AdminBrandApiClient] Lỗi cập nhật nhãn hiệu {Id}", maNhanHieu);
                return (false, "Không thể kết nối đến máy chủ");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteBrandAsync(string maNhanHieu)
        {
            try
            {
                var response = await _client.DeleteAsync($"api/nhan-hieu/{Uri.EscapeDataString(maNhanHieu)}");
                if (response.IsSuccessStatusCode) return (true, null);
                return (false, await AdminApiHelper.ReadErrorMessageAsync(response, "Không thể xóa nhãn hiệu."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AdminBrandApiClient] Lỗi xóa nhãn hiệu {Id}", maNhanHieu);
                return (false, "Không thể kết nối đến máy chủ");
            }
        }
    }
}

