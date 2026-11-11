using NexusRetail.Shared.DTOs.BanHang;
using System.Net.Http.Json;

namespace NexusRetail.WinForms.Services;

/// <summary>
/// Dịch vụ bán hàng POS, đối soát VietQR và tra cứu hóa đơn bán.
/// </summary>
public class PosSalesClient : PosApiClientBase, IPosSalesClient
{
    public PosSalesClient(PosSessionContext session) : base(session)
    {
    }

    public async Task<string?> GetNextInvoiceCodeAsync()
    {
        try
        {
            var res = await Client.GetFromJsonAsync<NextCodeResult>("api/ban-hang/next-ma-hdb");
            return string.IsNullOrWhiteSpace(res?.MaHdb) ? null : res.MaHdb;
        }
        catch
        {
            return null;
        }
    }

    public async Task<(bool Success, string Message, string? MaHdb, decimal TongTien)> CreateSaleOrderAsync(HoaDonBanDto dto)
    {
        try
        {
            var response = await Client.PostAsJsonAsync("api/ban-hang/thanh-toan", dto);
            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadFromJsonAsync<HoaDonBanResponseDto>();
                return (true, "Lập hóa đơn thành công!", res?.MaHdb, res?.TongTien ?? 0m);
            }

            return (false, await FormatApiErrorAsync(response, "Lỗi"), null, 0m);
        }
        catch (Exception ex)
        {
            return (false, $"Không thể kết nối máy chủ: {ex.Message}", null, 0m);
        }
    }

    public async Task<PaymentCheckResult> CheckBankPaymentStatusAsync(string maHdb, decimal? expectedAmount = null)
    {
        try
        {
            string url = $"api/webhook/bank/status/{Uri.EscapeDataString(maHdb)}";
            if (expectedAmount.HasValue && expectedAmount.Value > 0)
            {
                url += $"?expectedAmount={expectedAmount.Value}";
            }

            var res = await Client.GetFromJsonAsync<PaymentCheckResult>(url);
            return res ?? new PaymentCheckResult { IsPaid = false };
        }
        catch
        {
            return new PaymentCheckResult { IsPaid = false, Message = "Chờ máy chủ phản hồi..." };
        }
    }

    public Task<List<InvoiceSearchItem>> SearchSaleInvoicesAsync(string? q = null)
    {
        string url = "api/ban-hang/danh-sach?page=1&pageSize=100";
        if (!string.IsNullOrWhiteSpace(q))
            url += $"&q={Uri.EscapeDataString(q)}";

        return GetPropertyListAsync<InvoiceSearchItem>(url, "items");
    }

    public Task<List<InvoiceDetailItem>> GetSaleInvoiceDetailsAsync(string maHdb)
        => GetPropertyListAsync<InvoiceDetailItem>($"api/ban-hang/chi-tiet/{Uri.EscapeDataString(maHdb)}", "chiTiet");
}

