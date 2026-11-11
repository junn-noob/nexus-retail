using NexusRetail.Shared.DTOs.NhapHang;
using NexusRetail.WebMvc.Models;

namespace NexusRetail.WebMvc.Services.Admin
{
    public interface IAdminInboundApiClient
    {
        Task<List<AdminInboundInvoiceItem>> GetInboundInvoicesAsync();
        Task<HoaDonNhapResponseDto?> GetInboundInvoiceDetailAsync(string maHdn);
        Task<List<AdminSupplierOption>> GetSuppliersAsync();
        Task<List<AdminEmployeeOption>> GetEmployeesAsync();
        Task<string?> GetNextInboundCodeAsync();
        Task<(bool Success, string? MaHdn, string? ErrorMessage)> CreateInboundInvoiceAsync(HoaDonNhapDto dto);
    }
}

