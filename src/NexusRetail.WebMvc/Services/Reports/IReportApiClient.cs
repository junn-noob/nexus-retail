using System.Text.Json;

namespace NexusRetail.WebMvc.Services.Reports
{
    public record DashboardStats(int TotalProducts, int TotalOrders, decimal TotalRevenue, int TotalCustomers);

    public record DashboardChartsData(
        List<string> MonthlyLabels,
        List<decimal> MonthlyRevenuesMillions,
        List<string> BrandLabels,
        List<decimal> BrandPercentages);

    public interface IReportApiClient
    {
        Task<DashboardStats> GetDashboardStatsAsync();
        Task<DashboardChartsData> GetDashboardChartsAsync();
        Task<JsonElement?> GetReportDataAsync(string loai, int? thang = null, int? nam = null, int? quy = null);
        Task<byte[]?> DownloadExcelReportAsync(string loai, int? thang = null, int? nam = null, int? quy = null);
    }
}
