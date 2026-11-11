namespace NexusRetail.WinForms.Services;

/// <summary>
/// Dịch vụ truy vấn báo cáo thống kê doanh thu, tồn kho và xuất tệp Excel cho WinForms POS.
/// </summary>
public class PosReportClient : PosApiClientBase, IPosReportClient
{
    public PosReportClient(PosSessionContext session) : base(session)
    {
    }

    public Task<List<ReportSpBanChayItem>> GetReportSpBanChayAsync(int month, int year)
        => GetPropertyListAsync<ReportSpBanChayItem>($"api/bao-cao/sp-ban-chay?month={month}&year={year}", "topSanPham");

    public Task<List<ReportHdnLonNhatItem>> GetReportHdnLonNhatAsync(int quarter, int year)
        => GetPropertyListAsync<ReportHdnLonNhatItem>($"api/bao-cao/hdn-lon-nhat?quarter={quarter}&year={year}", "danhSachHoaDon");

    public Task<List<ReportDoanhSoNamItem>> GetReportDoanhSoNamAsync(int year)
        => GetPropertyListAsync<ReportDoanhSoNamItem>($"api/bao-cao/doanh-so-nam?year={year}", "danhSachHoaDon");

    public Task<List<ReportNvDoanhSoThapItem>> GetReportNvDoanhSoThapAsync(int month, int year)
        => GetPropertyListAsync<ReportNvDoanhSoThapItem>($"api/bao-cao/nv-doanh-so-thap?month={month}&year={year}", "danhSachNhanVien");

    public Task<List<ReportTonKhoItem>> GetReportTonKhoAsync(int month, int year)
        => GetPropertyListAsync<ReportTonKhoItem>($"api/bao-cao/ton-kho?month={month}&year={year}", "danhSach");

    public async Task<(bool Success, string Message)> DownloadExcelAsync(string relativeUrl, string saveFilePath)
    {
        try
        {
            var bytes = await Client.GetByteArrayAsync(relativeUrl);
            await File.WriteAllBytesAsync(saveFilePath, bytes);
            return (true, "Tải file Excel thành công!");
        }
        catch (Exception ex)
        {
            return (false, $"Không thể xuất file Excel: {ex.Message}");
        }
    }
}

