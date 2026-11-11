using NexusRetail.Shared.DTOs.BanHang;
using NexusRetail.WebMvc.Models;
using System.Text.Json;

namespace NexusRetail.WebMvc.Services.Orders
{
    public interface IOrderApiClient
    {
        Task<(bool Success, string Message, string? MaHdb, decimal TongTien)> CreateOrderAsync(HoaDonBanDto dto);
        Task<JsonElement?> GetOrdersByPhoneAsync(string phone);
        Task<(List<AdminOrderListItem> Items, int TotalPages, int TotalItems)> GetOrdersListAsync(int page = 1, int pageSize = 15, string? q = null);
        Task<AdminOrderDetailViewModel?> GetOrderDetailAsync(string id);
        Task<(bool Success, string Message)> UpdateOrderStatusAsync(string maHdb, CapNhatTrangThaiDonDto dto);
        Task<(bool IsPaid, decimal PaidAmount, decimal ExpectedAmount, string Message, string? TransactionCode)> GetBankPaymentStatusAsync(string maHdb, decimal? expectedAmount = null);
        Task<(bool Success, string Message)> SimulateBankPaymentAsync(string maHdb, decimal amount);
    }
}
