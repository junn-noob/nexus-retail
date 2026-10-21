using NexusRetail.Shared.DTOs.Common;
using NexusRetail.Shared.DTOs.SanPham;
using NexusRetail.WebMvc.Models;

namespace NexusRetail.WebMvc.Services.Catalog
{
    public class CatalogApiClient : ICatalogApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<CatalogApiClient> _logger;

        public CatalogApiClient(HttpClient client, ILogger<CatalogApiClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<PagedResultDto<SanPhamDto>> GetProductsAsync(int page = 1, int pageSize = 9, string? q = null, string? brand = null, string? price = null)
        {
            string url = $"api/san-pham?page={page}&pageSize={pageSize}";

            if (!string.IsNullOrWhiteSpace(brand))
                url += $"&brand={Uri.EscapeDataString(brand)}";

            if (!string.IsNullOrWhiteSpace(q))
                url += $"&q={Uri.EscapeDataString(q)}";

            if (!string.IsNullOrWhiteSpace(price))
            {
                url += price switch
                {
                    "lt20" => "&maxPrice=20000000",
                    "mid"  => "&minPrice=20000000&maxPrice=30000000",
                    "gt30" => "&minPrice=30000000",
                    _ => ""
                };
            }

            try
            {
                var data = await _client.GetFromJsonAsync<PagedResultDto<SanPhamDto>>(url);
                return data ?? new PagedResultDto<SanPhamDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CatalogApiClient] Lỗi khi lấy danh sách sản phẩm (url: {Url})", url);
                return new PagedResultDto<SanPhamDto>();
            }
        }

        public async Task<SanPhamDto?> GetProductByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;

            try
            {
                var response = await _client.GetAsync($"api/san-pham/{Uri.EscapeDataString(id.Trim())}");
                if (!response.IsSuccessStatusCode) return null;

                return await response.Content.ReadFromJsonAsync<SanPhamDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CatalogApiClient] Lỗi khi lấy chi tiết sản phẩm {Id}", id);
                return null;
            }
        }

        public async Task<PagedResultDto<SanPhamDto>> GetProductsByBrandAsync(string? maNhanHieu, int page = 1, int pageSize = 9)
        {
            try
            {
                string url = string.IsNullOrWhiteSpace(maNhanHieu)
                    ? $"api/san-pham?page={page}&pageSize={pageSize}"
                    : $"api/san-pham/nhan-hieu/{Uri.EscapeDataString(maNhanHieu)}?page={page}&pageSize={pageSize}";

                var data = await _client.GetFromJsonAsync<PagedResultDto<SanPhamDto>>(url);
                return data ?? new PagedResultDto<SanPhamDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CatalogApiClient] Lỗi lấy sản phẩm theo nhãn hiệu: {MaNhanHieu}", maNhanHieu);
                return new PagedResultDto<SanPhamDto>();
            }
        }

        public async Task<PagedResultDto<SanPhamDto>> GetCompareProductsAsync()
        {
            try
            {
                var response = await _client.GetAsync("api/san-pham?page=1&pageSize=100");
                if (!response.IsSuccessStatusCode) return new PagedResultDto<SanPhamDto>();

                var data = await response.Content.ReadFromJsonAsync<PagedResultDto<SanPhamDto>>();
                return data ?? new PagedResultDto<SanPhamDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CatalogApiClient] Lỗi nạp danh sách so sánh sản phẩm");
                return new PagedResultDto<SanPhamDto>();
            }
        }

        public async Task<CategoryFilterDto?> GetCategoryFilterAsync()
        {
            try
            {
                return await _client.GetFromJsonAsync<CategoryFilterDto>("api/san-pham/bo-loc-danh-muc");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[CatalogApiClient] Không thể lấy bộ lọc danh mục");
                return new CategoryFilterDto();
            }
        }
    }
}
