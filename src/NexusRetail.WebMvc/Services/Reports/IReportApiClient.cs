using System.Text.Json;

namespace NexusRetail.WebMvc.Services.Reports
{
    public record DashboardStats(int TotalProducts, int TotalOrders, decimal TotalRevenue, int TotalCustomers);

    public interface IReportApiClient
    {
        Task<DashboardStats> GetDashboardStatsAsync();
        Task<JsonElement?> GetReportDataAsync(string loai, int? thang = null, int? nam = null, int? quy = null);
        Task<byte[]?> DownloadExcelReportAsync(string loai, int? thang = null, int? nam = null, int? quy = null);
    }
}
