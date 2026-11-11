using NexusRetail.Shared.DTOs.NhapHang;
using System.Net.Http.Json;

namespace NexusRetail.WinForms.Services;

/// <summary>
/// Dịch vụ nhập kho từ nhà cung cấp và tra cứu phiếu nhập hàng cho WinForms POS.
/// </summary>
public class PosInboundClient : PosApiClientBase, IPosInboundClient
{
    public PosInboundClient(PosSessionContext session) : base(session)
    {
    }

    public async Task<List<NhaCungCapItem>> GetSuppliersAsync()
    {
        try
        {
            var list = await Client.GetFromJsonAsync<List<NhaCungCapItem>>("api/nhap-hang/nha-cung-cap");
            return list ?? GetDefaultSuppliers();
        }
        catch
        {
            return GetDefaultSuppliers();
        }
    }

    public async Task<List<NhanVienItem>> GetEmployeesAsync()
    {
        try
        {
            var list = await Client.GetFromJsonAsync<List<NhanVienItem>>("api/nhap-hang/nhan-vien");
            return list ?? GetDefaultEmployees();
        }
        catch
        {
            return GetDefaultEmployees();
        }
    }

    public async Task<string?> GetNextInboundInvoiceCodeAsync()
    {
        try
        {
            var res = await Client.GetFromJsonAsync<NextInboundCodeResult>("api/nhap-hang/next-ma-hdn");
            return string.IsNullOrWhiteSpace(res?.MaHdn) ? null : res.MaHdn;
        }
        catch
        {
            return null;
        }
    }

    public async Task<(bool Success, string Message, string? MaHdn, decimal TongTien)> CreateInboundOrderAsync(HoaDonNhapDto dto)
    {
        try
        {
            var response = await Client.PostAsJsonAsync("api/nhap-hang", dto);
            if (response.IsSuccessStatusCode)
            {
                var res = await response.Content.ReadFromJsonAsync<HoaDonNhapResponseDto>();
                return (true, "Lập hóa đơn nhập hàng thành công! Kho đã tự động cập nhật.", res?.MaHdn, res?.TongTien ?? 0m);
            }

            return (false, await FormatApiErrorAsync(response, "Lỗi nhập hàng"), null, 0m);
        }
        catch (Exception ex)
        {
            return (false, $"Lỗi kết nối máy chủ: {ex.Message}", null, 0m);
        }
    }

    public async Task<List<InboundInvoiceSearchItem>> SearchInboundInvoicesAsync()
    {
        try
        {
            return await Client.GetFromJsonAsync<List<InboundInvoiceSearchItem>>("api/nhap-hang") ?? new();
        }
        catch
        {
            return new();
        }
    }

    public async Task<List<InvoiceDetailItem>> GetInboundInvoiceDetailsAsync(string maHdn)
    {
        try
        {
            var res = await Client.GetFromJsonAsync<HoaDonNhapResponseDto>($"api/nhap-hang/{Uri.EscapeDataString(maHdn)}");
            if (res?.SanPham != null)
            {
                return res.SanPham.Select(x => new InvoiceDetailItem
                {
                    MaSp = x.MaSp,
                    TenSp = x.TenSp ?? x.MaSp,
                    SoLuong = x.SoLuong,
                    DonGia = x.DonGia,
                    KhuyenMai = x.KhuyenMai,
                    ThanhTien = x.ThanhTien
                }).ToList();
            }
        }
        catch { }

        return new List<InvoiceDetailItem>();
    }

    private static List<NhaCungCapItem> GetDefaultSuppliers() =>
    [
        new() { MaNcc = "NCC01", TenNcc = "Apple Việt Nam - Synnex FPT", DiaChi = "Tòa nhà FPT Cầu Giấy, Quận Cầu Giấy, Hà Nội", DienThoai = "19006600" },
        new() { MaNcc = "NCC02", TenNcc = "Samsung Electronics Việt Nam", DiaChi = "KCN Yên Phong, Bắc Ninh", DienThoai = "1800588889" },
        new() { MaNcc = "NCC03", TenNcc = "Xiaomi Mi Home Distribution", DiaChi = "Quận Cầu Giấy, Hà Nội", DienThoai = "19001900" }
    ];

    private static List<NhanVienItem> GetDefaultEmployees() =>
    [
        new() { MaNv = "NV01", TenNv = "Nguyễn Văn Hùng", DienThoai = "0988888888" },
        new() { MaNv = "NV02", TenNv = "Trần Thị Mai", DienThoai = "0977123456" },
        new() { MaNv = "NV03", TenNv = "Lê Hoàng Long", DienThoai = "0912345678" }
    ];
}

