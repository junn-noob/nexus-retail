using NexusRetail.Shared.DTOs.Common;
using NexusRetail.Shared.DTOs.SanPham;
using System.Text.Json;

namespace NexusRetail.WebMvc.Services.Reports
{
    public class ReportApiClient : IReportApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<ReportApiClient> _logger;

        public ReportApiClient(HttpClient client, ILogger<ReportApiClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            int totalProducts = 0;
            int totalOrders = 0;
            decimal totalRevenue = 0m;
            int totalCustomers = 0;

            try
            {
                var statsRes = await _client.GetFromJsonAsync<JsonElement>("api/ban-hang/thong-ke-dashboard");
                if (statsRes.TryGetProperty("totalProducts", out var tp)) totalProducts = tp.GetInt32();
                if (statsRes.TryGetProperty("totalOrders", out var to)) totalOrders = to.GetInt32();
                if (statsRes.TryGetProperty("totalRevenue", out var tr)) totalRevenue = tr.GetDecimal();
                if (statsRes.TryGetProperty("totalCustomers", out var tc)) totalCustomers = tc.GetInt32();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[ReportApiClient] Không thể gọi api/ban-hang/thong-ke-dashboard, dùng các API riêng lẻ");

                try
                {
                    var spRes = await _client.GetFromJsonAsync<PagedResultDto<SanPhamDto>>("api/san-pham?page=1&pageSize=1");
                    totalProducts = spRes?.TotalItems ?? 0;
                }
                catch (Exception exSp)
                {
                    _logger.LogError(exSp, "[ReportApiClient] Lỗi nạp tổng số sản phẩm");
                }

                try
                {
                    var dhRes = await _client.GetFromJsonAsync<JsonElement>("api/ban-hang/danh-sach?page=1&pageSize=1");
                    if (dhRes.TryGetProperty("totalItems", out var ti)) totalOrders = ti.GetInt32();
                }
                catch (Exception exDh)
                {
                    _logger.LogError(exDh, "[ReportApiClient] Lỗi nạp tổng số đơn hàng");
                }
            }

            return new DashboardStats(totalProducts, totalOrders, totalRevenue, totalCustomers);
        }

        public async Task<JsonElement?> GetReportDataAsync(string loai, int? thang = null, int? nam = null, int? quy = null)
        {
            nam ??= 2026;
            thang ??= 9;
            quy ??= 3;

            string apiUrl = loai switch
            {
                "sp-ban-chay" => $"api/bao-cao/sp-ban-chay?month={thang}&year={nam}",
                "nhap-lon-nhat-quy" => $"api/bao-cao/nhap-lon-nhat-quy?quy={quy}&year={nam}",
                "doanh-so-nam" => $"api/bao-cao/doanh-so-nam?year={nam}",
                "nv-ban-it-nhat" => $"api/bao-cao/nv-ban-it-nhat?month={thang}&year={nam}",
                _ => $"api/bao-cao/sp-ban-chay?month={thang}&year={nam}"
            };

            try
            {
                return await _client.GetFromJsonAsync<JsonElement>(apiUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ReportApiClient] Lỗi lấy dữ liệu báo cáo: {Url}", apiUrl);
                return null;
            }
        }

        public async Task<byte[]?> DownloadExcelReportAsync(string loai, int? thang = null, int? nam = null, int? quy = null)
        {
            nam ??= 2026;
            thang ??= 9;
            quy ??= 3;

            string excelUrl = loai switch
            {
                "sp-ban-chay" => $"api/bao-cao/sp-ban-chay/excel?month={thang}&year={nam}",
                "nhap-lon-nhat-quy" => $"api/bao-cao/nhap-lon-nhat-quy/excel?quy={quy}&year={nam}",
                "doanh-so-nam" => $"api/bao-cao/doanh-so-nam/excel?year={nam}",
                "nv-ban-it-nhat" => $"api/bao-cao/nv-ban-it-nhat/excel?month={thang}&year={nam}",
                _ => $"api/bao-cao/sp-ban-chay/excel?month={thang}&year={nam}"
            };

            try
            {
                var response = await _client.GetAsync(excelUrl);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ReportApiClient] Lỗi xuất Excel: {Url}", excelUrl);
                return null;
            }
        }
    }
}
