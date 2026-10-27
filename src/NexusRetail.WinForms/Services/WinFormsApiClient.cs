using NexusRetail.Shared.DTOs.Auth;
using NexusRetail.Shared.DTOs.BanHang;
using NexusRetail.Shared.DTOs.Common;
using NexusRetail.Shared.DTOs.SanPham;
using System.Net.Http.Json;
using System.Text.Json;

namespace NexusRetail.WinForms.Services
{
    public class PaymentCheckResult
    {
        public string MaHdb { get; set; } = "";
        public bool IsPaid { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal ExpectedAmount { get; set; }
        public string? TransactionCode { get; set; }
        public DateTime? PaidAt { get; set; }
        public string Message { get; set; } = "";
    }

    public class NextCodeResult
    {
        public string MaHdb { get; set; } = "";
    }

    public class NextInboundCodeResult
    {
        public string MaHdn { get; set; } = "";
    }

    public class WinFormsApiClient
    {
        private static readonly Lazy<WinFormsApiClient> _instance = new(() => new WinFormsApiClient());
        public static WinFormsApiClient Instance => _instance.Value;

        private HttpClient _client;
        private string? _token;
        public string BaseUrl { get; set; } = "https://localhost:7143/";
        public string? CurrentUser { get; set; }
        public string? CurrentRole { get; set; }

        private WinFormsApiClient()
        {
            _client = CreateHttpClient(BaseUrl);
        }

        private static HttpClient CreateHttpClient(string baseUrl)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            return new HttpClient(handler)
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromSeconds(15)
            };
        }

        public void SwitchPort(string newBaseUrl)
        {
            if (BaseUrl != newBaseUrl)
            {
                BaseUrl = newBaseUrl;
                _client = CreateHttpClient(newBaseUrl);
                if (!string.IsNullOrEmpty(_token))
                {
                    _client.DefaultRequestHeaders.Authorization = 
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
                }
            }
        }

        public void Logout()
        {
            CurrentUser = null;
            CurrentRole = null;
            _token = null;
            _client.DefaultRequestHeaders.Authorization = null;
        }

        private static readonly string[] AuthCandidateUrls = [
            "http://localhost:5001/",  // Identity.Api direct
            "http://localhost:5000/",  // Gateway
            "https://localhost:7143/", // Core.Api (monolith fallback)
            "http://localhost:5050/"   // Core.Api HTTP (monolith fallback)
        ];

        // 1. Xác thực tài khoản (hỗ trợ gọi trực tiếp Identity.Api hoặc thông qua Gateway)
        public async Task<(bool Success, string Role, string Message)> LoginAsync(string username, string password)
        {
            string lastError = "Tên đăng nhập hoặc mật khẩu không chính xác!";
            bool reachedAnyAuthService = false;

            foreach (var authUrl in AuthCandidateUrls)
            {
                try
                {
                    using var authClient = CreateHttpClient(authUrl);
                    var response = await authClient.PostAsJsonAsync("api/auth/login", new { username, password });
                    
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        // Endpoint này không có route api/auth/login (ví dụ Core.Api), thử tiếp candidate url khác
                        continue;
                    }

                    reachedAnyAuthService = true;

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                        CurrentUser = result?.Username ?? username;
                        CurrentRole = result?.Role ?? "Staff";
                        _token = result?.Token;

                        if (!string.IsNullOrEmpty(_token))
                        {
                            _client.DefaultRequestHeaders.Authorization = 
                                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
                        }

                        // Đồng thời tự động phát hiện và kết nối cổng Core API đang chạy (7143, 5050 hoặc 5000)
                        await EnsureCoreApiConnectedAsync();

                        return (true, CurrentRole, "Đăng nhập thành công!");
                    }

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return (false, "", "Tên đăng nhập hoặc mật khẩu không chính xác!");
                    }

                    try
                    {
                        var errObj = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
                        if (errObj.TryGetProperty("message", out var m))
                        {
                            return (false, "", m.GetString() ?? lastError);
                        }
                    }
                    catch { }

                    return (false, "", lastError);
                }
                catch
                {
                    // Lỗi kết nối cổng này, thử candidate tiếp theo
                }
            }

            if (!reachedAnyAuthService)
            {
                return (false, "", "Không thể kết nối đến máy chủ xác thực Identity (Port 5001) hoặc Gateway (Port 5000)!\nVui lòng kiểm tra dịch vụ NexusRetail.Identity.Api đã được khởi động chưa.");
            }

            return (false, "", lastError);
        }

        private async Task EnsureCoreApiConnectedAsync()
        {
            string[] coreCandidateUrls = ["https://localhost:7143/", "http://localhost:5050/", "http://localhost:5000/"];
            foreach (var url in coreCandidateUrls)
            {
                try
                {
                    using var testClient = CreateHttpClient(url);
                    var testRes = await testClient.GetAsync("api/san-pham?page=1&pageSize=1");
                    if (testRes.IsSuccessStatusCode)
                    {
                        SwitchPort(url);
                        break;
                    }
                }
                catch { }
            }
        }

        // 2. Danh mục sản phẩm
        public async Task<List<SanPhamDto>> GetProductsAsync(string? q = null)
        {
            string url = "api/san-pham?page=1&pageSize=100";
            if (!string.IsNullOrWhiteSpace(q))
                url += $"&q={Uri.EscapeDataString(q)}";

            try
            {
                var res = await _client.GetFromJsonAsync<PagedResultDto<SanPhamDto>>(url);
                return res?.Items ?? new List<SanPhamDto>();
            }
            catch
            {
                if (BaseUrl.Contains("7143"))
                {
                    try
                    {
                        using var altClient = CreateHttpClient("http://localhost:5050/");
                        var res = await altClient.GetFromJsonAsync<PagedResultDto<SanPhamDto>>(url);
                        if (res?.Items != null && res.Items.Count > 0)
                        {
                            SwitchPort("http://localhost:5050/");
                            return res.Items;
                        }
                    }
                    catch { }
                }

                return new List<SanPhamDto>();
            }
        }

        public async Task<(bool Success, string Message, string? MaSp)> CreateProductAsync(ThemSanPhamDto dto)
        {
            try
            {
                var response = await _client.PostAsJsonAsync("api/san-pham", dto);
                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadFromJsonAsync<JsonElement>();
                    string? maSp = res.TryGetProperty("maSp", out var m) ? m.GetString() : null;
                    return (true, "Thêm mới sản phẩm thành công!", maSp);
                }

                var err = await response.Content.ReadAsStringAsync();
                return (false, $"Lỗi từ máy chủ: {err}", null);
            }
            catch (Exception ex)
            {
                return (false, $"Không thể kết nối đến máy chủ: {ex.Message}", null);
            }
        }

        public async Task<(bool Success, string Message)> UpdateProductAsync(string maSp, SuaSanPhamDto dto)
        {
            try
            {
                var response = await _client.PutAsJsonAsync($"api/san-pham/{Uri.EscapeDataString(maSp)}", dto);
                if (response.IsSuccessStatusCode)
                {
                    return (true, "Cập nhật sản phẩm thành công!");
                }

                var err = await response.Content.ReadAsStringAsync();
                return (false, $"Lỗi cập nhật: {err}");
            }
            catch (Exception ex)
            {
                return (false, $"Không thể kết nối đến máy chủ: {ex.Message}");
            }
        }

        public async Task<(bool Success, string Message)> DeleteProductAsync(string maSp)
        {
            try
            {
                var response = await _client.DeleteAsync($"api/san-pham/{Uri.EscapeDataString(maSp)}");
                if (response.IsSuccessStatusCode)
                {
                    return (true, "Xóa sản phẩm thành công!");
                }

                var err = await response.Content.ReadAsStringAsync();
                return (false, $"Không thể xóa: {err}");
            }
            catch (Exception ex)
            {
                return (false, $"Không thể kết nối đến máy chủ: {ex.Message}");
            }
        }

        // 3. Tạo hóa đơn bán hàng
        public async Task<(bool Success, string Message, string? MaHdb, decimal TongTien)> CreateSaleOrderAsync(HoaDonBanDto dto)
        {
            try
            {
                var response = await _client.PostAsJsonAsync("api/ban-hang/thanh-toan", dto);
                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadFromJsonAsync<HoaDonBanResponseDto>();
                    return (true, "Lập hóa đơn thành công!", res?.MaHdb, res?.TongTien ?? 0m);
                }

                var err = await response.Content.ReadAsStringAsync();
                return (false, $"Lỗi: {err}", null, 0m);
            }
            catch (Exception ex)
            {
                return (false, $"Không thể kết nối máy chủ: {ex.Message}", null, 0m);
            }
        }

        // 4. KIỂM TRA TRẠNG THÁI WEBHOOK NGÂN HÀNG (KÈM SO SÁNH SỐ TIỀN)
        public async Task<PaymentCheckResult> CheckBankPaymentStatusAsync(string maHdb, decimal? expectedAmount = null)
        {
            try
            {
                string url = $"api/webhook/bank/status/{Uri.EscapeDataString(maHdb)}";
                if (expectedAmount.HasValue && expectedAmount.Value > 0)
                {
                    url += $"?expectedAmount={expectedAmount.Value}";
                }
                var res = await _client.GetFromJsonAsync<PaymentCheckResult>(url);
                return res ?? new PaymentCheckResult { IsPaid = false };
            }
            catch
            {
                return new PaymentCheckResult { IsPaid = false, Message = "Chờ máy chủ phản hồi..." };
            }
        }

        // 5. CỔNG SANDBOX / TEST GATEWAY
        public async Task<(bool Success, string Message)> SimulateBankPaymentAsync(string maHdb, decimal amount)
        {
            try
            {
                var response = await _client.PostAsJsonAsync("api/webhook/bank/simulate", new { MaHdb = maHdb, Amount = amount });
                return (response.IsSuccessStatusCode, "Đã gửi tín hiệu xác nhận Sandbox MBBank!");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        // Lấy mã hóa đơn bán kế tiếp chuẩn từ Server (Ví dụ: HDB13)
        public async Task<string?> GetNextInvoiceCodeAsync()
        {
            try
            {
                var res = await _client.GetFromJsonAsync<NextCodeResult>("api/ban-hang/next-ma-hdb");
                if (!string.IsNullOrWhiteSpace(res?.MaHdb))
                {
                    return res.MaHdb;
                }
            }
            catch { }
            return null;
        }

        // Lấy mã hóa đơn nhập kế tiếp chuẩn từ Server (Ví dụ: HDN04)
        public async Task<string?> GetNextInboundInvoiceCodeAsync()
        {
            try
            {
                var res = await _client.GetFromJsonAsync<NextInboundCodeResult>("api/nhap-hang/next-ma-hdn");
                if (!string.IsNullOrWhiteSpace(res?.MaHdn))
                {
                    return res.MaHdn;
                }
            }
            catch { }
            return null;
        }

        // 6. NHẬP HÀNG - LẤY NHÀ CUNG CẤP & NHÂN VIÊN
        public async Task<List<NhaCungCapItem>> GetSuppliersAsync()
        {
            try
            {
                var list = await _client.GetFromJsonAsync<List<NhaCungCapItem>>("api/nhap-hang/nha-cung-cap");
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
                var list = await _client.GetFromJsonAsync<List<NhanVienItem>>("api/nhap-hang/nhan-vien");
                return list ?? GetDefaultEmployees();
            }
            catch
            {
                return GetDefaultEmployees();
            }
        }

        public async Task<(bool Success, string Message, string? MaHdn, decimal TongTien)> CreateInboundOrderAsync(NexusRetail.Shared.DTOs.NhapHang.HoaDonNhapDto dto)
        {
            try
            {
                var response = await _client.PostAsJsonAsync("api/nhap-hang", dto);
                if (response.IsSuccessStatusCode)
                {
                    var res = await response.Content.ReadFromJsonAsync<NexusRetail.Shared.DTOs.NhapHang.HoaDonNhapResponseDto>();
                    return (true, "Lập hóa đơn nhập hàng thành công! Kho đã tự động cập nhật.", res?.MaHdn, res?.TongTien ?? 0m);
                }

                var err = await response.Content.ReadAsStringAsync();
                return (false, $"Lỗi nhập hàng: {err}", null, 0m);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi kết nối máy chủ: {ex.Message}", null, 0m);
            }
        }

        // 7. BÁO CÁO THỐNG KÊ (BR-06, BR-07, BR-08, BR-09)
        public async Task<List<ReportSpBanChayItem>> GetReportSpBanChayAsync(int month, int year)
        {
            try
            {
                using var res = await _client.GetAsync($"api/bao-cao/sp-ban-chay?month={month}&year={year}");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("topSanPham", out var arr))
                    {
                        return JsonSerializer.Deserialize<List<ReportSpBanChayItem>>(arr.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                    }
                }
            }
            catch { }
            return new List<ReportSpBanChayItem>();
        }

        public async Task<List<ReportHdnLonNhatItem>> GetReportHdnLonNhatAsync(int quarter, int year)
        {
            try
            {
                using var res = await _client.GetAsync($"api/bao-cao/hdn-lon-nhat?quarter={quarter}&year={year}");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("danhSachHoaDon", out var arr))
                    {
                        return JsonSerializer.Deserialize<List<ReportHdnLonNhatItem>>(arr.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                    }
                }
            }
            catch { }
            return new List<ReportHdnLonNhatItem>();
        }

        public async Task<List<ReportDoanhSoNamItem>> GetReportDoanhSoNamAsync(int year)
        {
            try
            {
                using var res = await _client.GetAsync($"api/bao-cao/doanh-so-nam?year={year}");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("danhSachHoaDon", out var arr))
                    {
                        return JsonSerializer.Deserialize<List<ReportDoanhSoNamItem>>(arr.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                    }
                }
            }
            catch { }
            return new List<ReportDoanhSoNamItem>();
        }

        public async Task<List<ReportNvDoanhSoThapItem>> GetReportNvDoanhSoThapAsync(int month, int year)
        {
            try
            {
                using var res = await _client.GetAsync($"api/bao-cao/nv-doanh-so-thap?month={month}&year={year}");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("danhSachNhanVien", out var arr))
                    {
                        return JsonSerializer.Deserialize<List<ReportNvDoanhSoThapItem>>(arr.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                    }
                }
            }
            catch { }
            return new List<ReportNvDoanhSoThapItem>();
        }

        public async Task<(bool Success, string Message)> DownloadExcelAsync(string relativeUrl, string saveFilePath)
        {
            try
            {
                var bytes = await _client.GetByteArrayAsync(relativeUrl);
                await File.WriteAllBytesAsync(saveFilePath, bytes);
                return (true, "Tải file Excel thành công!");
            }
            catch (Exception ex)
            {
                return (false, $"Không thể xuất file Excel: {ex.Message}");
            }
        }

        // 8. TÌM KIẾM HÓA ĐƠN BÁN & HÓA ĐƠN NHẬP (YÊU CẦU 5)
        public async Task<List<InvoiceSearchItem>> SearchSaleInvoicesAsync(string? q = null)
        {
            try
            {
                string url = "api/ban-hang/danh-sach?page=1&pageSize=100";
                if (!string.IsNullOrWhiteSpace(q))
                    url += $"&q={Uri.EscapeDataString(q)}";

                using var res = await _client.GetAsync(url);
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("items", out var arr))
                    {
                        return JsonSerializer.Deserialize<List<InvoiceSearchItem>>(arr.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                    }
                }
            }
            catch { }
            return new List<InvoiceSearchItem>();
        }

        public async Task<List<InvoiceDetailItem>> GetSaleInvoiceDetailsAsync(string maHdb)
        {
            try
            {
                using var res = await _client.GetAsync($"api/ban-hang/chi-tiet/{Uri.EscapeDataString(maHdb)}");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("chiTiet", out var arr))
                    {
                        return JsonSerializer.Deserialize<List<InvoiceDetailItem>>(arr.GetRawText(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                    }
                }
            }
            catch { }
            return new List<InvoiceDetailItem>();
        }

        public async Task<List<InboundInvoiceSearchItem>> SearchInboundInvoicesAsync()
        {
            try
            {
                var list = await _client.GetFromJsonAsync<List<InboundInvoiceSearchItem>>("api/nhap-hang");
                return list ?? new();
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
                var res = await _client.GetFromJsonAsync<NexusRetail.Shared.DTOs.NhapHang.HoaDonNhapResponseDto>($"api/nhap-hang/{Uri.EscapeDataString(maHdn)}");
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


        private static List<NhaCungCapItem> GetDefaultSuppliers() => new()
        {
            new() { MaNcc = "NCC01", TenNcc = "Apple Việt Nam - Synnex FPT", DiaChi = "Tòa nhà FPT Cầu Giấy, Hà Nội", DienThoai = "19006600" },
            new() { MaNcc = "NCC02", TenNcc = "Samsung Electronics Việt Nam", DiaChi = "KCN Yên Phong, Bắc Ninh", DienThoai = "1800588889" },
            new() { MaNcc = "NCC03", TenNcc = "Xiaomi Mi Home Distribution", DiaChi = "Quận 1, TP Hồ Chí Minh", DienThoai = "19001900" }
        };

        private static List<NhanVienItem> GetDefaultEmployees() => new()
        {
            new() { MaNv = "NV01", TenNv = "Nguyễn Văn Hùng", DienThoai = "0988888888" },
            new() { MaNv = "NV02", TenNv = "Trần Thị Mai", DienThoai = "0977123456" },
            new() { MaNv = "NV03", TenNv = "Lê Hoàng Long", DienThoai = "0912345678" }
        };
    }

    public class NhaCungCapItem
    {
        public string MaNcc { get; set; } = "";
        public string TenNcc { get; set; } = "";
        public string? DiaChi { get; set; }
        public string? DienThoai { get; set; }
        public override string ToString() => $"{MaNcc} - {TenNcc}";
    }

    public class NhanVienItem
    {
        public string MaNv { get; set; } = "";
        public string TenNv { get; set; } = "";
        public string? DienThoai { get; set; }
        public override string ToString() => $"{MaNv} - {TenNv}";
    }

    public class ReportSpBanChayItem
    {
        public string MaSp { get; set; } = "";
        public string TenSp { get; set; } = "";
        public string TenLoai { get; set; } = "";
        public string TenNhanHieu { get; set; } = "";
        public int TongSoLuong { get; set; }
        public decimal TongDoanhThu { get; set; }
    }

    public class ReportHdnLonNhatItem
    {
        public string MaHdn { get; set; } = "";
        public DateTime NgayNhap { get; set; }
        public string TenNcc { get; set; } = "";
        public string TenNv { get; set; } = "";
        public decimal TongTien { get; set; }
    }

    public class ReportDoanhSoNamItem
    {
        public string MaHdb { get; set; } = "";
        public DateTime NgayBan { get; set; }
        public string TenKhach { get; set; } = "";
        public string TenNv { get; set; } = "";
        public decimal TongTien { get; set; }
    }

    public class ReportNvDoanhSoThapItem
    {
        public string MaNv { get; set; } = "";
        public string TenNv { get; set; } = "";
        public string GioiTinh { get; set; } = "";
        public string DienThoai { get; set; } = "";
        public decimal TongTienBan { get; set; }
    }

    public class InvoiceSearchItem
    {
        public string MaHdb { get; set; } = "";
        public DateTime NgayBan { get; set; }
        public decimal TongTien { get; set; }
        public string TenKhach { get; set; } = "";
        public string DienThoai { get; set; } = "";
        public string TenNv { get; set; } = "";
    }

    public class InboundInvoiceSearchItem
    {
        public string MaHdn { get; set; } = "";
        public DateTime NgayNhap { get; set; }
        public decimal TongTien { get; set; }
        public string MaNcc { get; set; } = "";
        public string TenNcc { get; set; } = "";
        public string MaNv { get; set; } = "";
        public string TenNv { get; set; } = "";
        public int SoMatHang { get; set; }
    }

    public class InvoiceDetailItem
    {
        public string MaSp { get; set; } = "";
        public string TenSp { get; set; } = "";
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal KhuyenMai { get; set; }
        public decimal ThanhTien { get; set; }
    }
}


