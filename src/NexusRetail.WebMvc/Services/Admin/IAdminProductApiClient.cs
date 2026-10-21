using NexusRetail.Shared.DTOs.SanPham;

namespace NexusRetail.WebMvc.Services.Admin
{
    public interface IAdminProductApiClient
    {
        Task<(bool Success, string? ErrorMessage)> CreateProductAsync(ThemSanPhamDto dto);
        Task<(bool Success, string? ErrorMessage)> UpdateProductAsync(string id, SuaSanPhamDto dto);
        Task<(bool Success, string? ErrorMessage)> DeleteProductAsync(string id);
    }
}
