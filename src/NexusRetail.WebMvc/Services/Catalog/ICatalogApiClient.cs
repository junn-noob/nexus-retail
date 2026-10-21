using NexusRetail.Shared.DTOs.Common;
using NexusRetail.Shared.DTOs.SanPham;
using NexusRetail.WebMvc.Models;

namespace NexusRetail.WebMvc.Services.Catalog
{
    public interface ICatalogApiClient
    {
        Task<PagedResultDto<SanPhamDto>> GetProductsAsync(int page = 1, int pageSize = 9, string? q = null, string? brand = null, string? price = null);
        Task<SanPhamDto?> GetProductByIdAsync(string id);
        Task<PagedResultDto<SanPhamDto>> GetProductsByBrandAsync(string? maNhanHieu, int page = 1, int pageSize = 9);
        Task<PagedResultDto<SanPhamDto>> GetCompareProductsAsync();
        Task<CategoryFilterDto?> GetCategoryFilterAsync();
    }
}
