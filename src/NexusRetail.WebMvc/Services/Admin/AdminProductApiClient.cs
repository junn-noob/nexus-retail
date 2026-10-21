using NexusRetail.Shared.DTOs.SanPham;

namespace NexusRetail.WebMvc.Services.Admin
{
    public class AdminProductApiClient : IAdminProductApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<AdminProductApiClient> _logger;

        public AdminProductApiClient(HttpClient client, ILogger<AdminProductApiClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateProductAsync(ThemSanPhamDto dto)
        {
            try
            {
                var response = await _client.PostAsJsonAsync("api/san-pham", dto);
                if (response.IsSuccessStatusCode)
                    return (true, null);

                var err = await response.Content.ReadAsStringAsync();
                return (false, err);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AdminProductApiClient] Lỗi thêm sản phẩm");
                return (false, "Không thể kết nối đến máy chủ");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateProductAsync(string id, SuaSanPhamDto dto)
        {
            try
            {
                var response = await _client.PutAsJsonAsync($"api/san-pham/{Uri.EscapeDataString(id)}", dto);
                if (response.IsSuccessStatusCode)
                    return (true, null);

                var err = await response.Content.ReadAsStringAsync();
                return (false, err);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AdminProductApiClient] Lỗi cập nhật sản phẩm {Id}", id);
                return (false, "Không thể kết nối đến máy chủ");
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteProductAsync(string id)
        {
            try
            {
                var response = await _client.DeleteAsync($"api/san-pham/{Uri.EscapeDataString(id)}");
                if (response.IsSuccessStatusCode)
                    return (true, null);

                return (false, "Không thể xóa sản phẩm đã phát sinh hóa đơn bán hoặc hóa đơn nhập!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AdminProductApiClient] Lỗi xóa sản phẩm {Id}", id);
                return (false, "Lỗi kết nối máy chủ");
            }
        }
    }
}
