using NexusRetail.WebMvc.Models;

namespace NexusRetail.WebMvc.Services.Admin
{
    public interface IAdminBrandApiClient
    {
        Task<List<BrandOption>> GetBrandsAsync();
        Task<(bool Success, string? ErrorMessage)> CreateBrandAsync(string tenNhanHieu);
        Task<(bool Success, string? ErrorMessage)> UpdateBrandAsync(string maNhanHieu, string tenNhanHieu);
        Task<(bool Success, string? ErrorMessage)> DeleteBrandAsync(string maNhanHieu);
    }
}

