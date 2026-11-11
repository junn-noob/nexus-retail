using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using NexusRetail.Core.Api.BUS;
using NexusRetail.Core.Api.Controllers;
using NexusRetail.Core.Api.Data;
using NexusRetail.Core.Api.Services.Export;
using NexusRetail.Shared.DTOs.BanHang;
using NexusRetail.Shared.DTOs.Common;
using NexusRetail.Shared.DTOs.SanPham;
using NexusRetail.Shared.Entities;
using NexusRetail.WebMvc.Controllers;
using NexusRetail.WebMvc.Models;
using NexusRetail.WebMvc.Services.Catalog;
using NexusRetail.WebMvc.Services.Orders;
using Xunit;

namespace NexusRetail.UnitTests
{
    public class UpgradeRegressionTests
    {
        private static RetailDbContext CreateTestDb()
        {
            var options = new DbContextOptionsBuilder<RetailDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            var db = new RetailDbContext(options);
            db.Loais.Add(new Loai { MaLoai = "L01", TenLoai = "Flagship" });
            db.NhanHieus.Add(new NhanHieu { MaNhanHieu = "NH01", TenNhanHieu = "Apple" });
            db.ManHinhs.Add(new ManHinh { MaManHinh = "MH01", TenManHinh = "OLED 6.7 inch" });
            db.NhanViens.Add(new NhanVien { MaNv = "NV01", TenNv = "Nhân Viên A", GioiTinh = "Nam", DienThoai = "0900111222" });
            db.KhachHangs.Add(new KhachHang { MaKhach = "KH01", TenKhach = "Khách Test", DienThoai = "0911222333" });
            db.SaveChanges();
            return db;
        }

        [Fact]
        public void ChungTuBase_InheritanceAndPolymorphism_WorksCorrectly()
        {
            ChungTuBase saleDoc = new HoaDonBan
            {
                MaHdb = "HDB01",
                MaNv = "NV01",
                MaKhach = "KH01",
                NgayBan = DateTime.UtcNow,
                TongTien = 20_000_000m
            };

            ChungTuBase inboundDoc = new HoaDonNhap
            {
                MaHdn = "HDN01",
                MaNv = "NV01",
                MaNcc = "NCC01",
                NgayNhap = DateTime.UtcNow,
                TongTien = 50_000_000m
            };

            Assert.Equal("Hóa đơn bán lẻ", saleDoc.LoaiChungTu);
            Assert.Equal("Phiếu nhập kho", inboundDoc.LoaiChungTu);
            Assert.Equal(2_000_000m, saleDoc.TinhThueVat());
            Assert.Equal(5_000_000m, inboundDoc.TinhThueVat());
        }

        [Fact]
        public void CoreControllers_AuthorizationPolicies_MatchSecurityContract()
        {
            // Report & Inbound controllers require Admin / Staff
            var baoCaoAuth = typeof(BaoCaoController).GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(baoCaoAuth);
            Assert.Equal("Admin", baoCaoAuth!.Roles);

            var nhapHangAuth = typeof(NhapHangController).GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(nhapHangAuth);
            Assert.Equal("Admin,Staff", nhapHangAuth!.Roles);

            // SanPham: public reads allow anonymous, delete strictly Admin, create/update Admin or Staff
            Assert.NotNull(typeof(SanPhamController).GetMethod(nameof(SanPhamController.layDanhSach))?.GetCustomAttribute<AllowAnonymousAttribute>());
            var themSpAuth = typeof(SanPhamController).GetMethod(nameof(SanPhamController.themSanPham))?.GetCustomAttribute<AuthorizeAttribute>();
            var xoaSpAuth = typeof(SanPhamController).GetMethod(nameof(SanPhamController.xoaSanPham))?.GetCustomAttribute<AuthorizeAttribute>();
            Assert.Equal("Admin,Staff", themSpAuth?.Roles);
            Assert.Equal("Admin", xoaSpAuth?.Roles);

            // NhanHieu: public read, Admin-only delete
            Assert.NotNull(typeof(NhanHieuController).GetMethod(nameof(NhanHieuController.layTatCa))?.GetCustomAttribute<AllowAnonymousAttribute>());
            var xoaNhAuth = typeof(NhanHieuController).GetMethod(nameof(NhanHieuController.xoaNhanHieu))?.GetCustomAttribute<AuthorizeAttribute>();
            Assert.Equal("Admin", xoaNhAuth?.Roles);

            // BanHang: checkout/tracking public, internal order list requires Admin or Staff
            Assert.NotNull(typeof(BanHangController).GetMethod(nameof(BanHangController.thanhToan))?.GetCustomAttribute<AllowAnonymousAttribute>());
            var dsHdbAuth = typeof(BanHangController).GetMethod(nameof(BanHangController.layDanhSach))?.GetCustomAttribute<AuthorizeAttribute>();
            Assert.Equal("Admin,Staff", dsHdbAuth?.Roles);
        }

        [Fact]
        public async Task SanPhamBus_CreateAndSearch_ComputesPriceAndFindsMatch()
        {
            using var db = CreateTestDb();
            var bus = new SanPhamBUS(db);

            var created = await bus.ThemSanPhamAsync(new ThemSanPhamDto
            {
                TenSp = "iPhone 16 Pro Max",
                MaLoai = "L01",
                MaNhanHieu = "NH01",
                MaManHinh = "MH01",
                GiaNhap = 30_000_000m,
                SoLuong = 3,
                ThoiGianBaoHanh = 12
            });

            Assert.True(created.IsSuccess);
            Assert.NotNull(created.Data);
            Assert.Equal(33_000_000m, created.Data!.GiaBan);

            var searchResults = await bus.TimKiemAsync("16 Pro", null, 30_000_000m, 35_000_000m);
            Assert.Single(searchResults);
            Assert.Equal("iPhone 16 Pro Max", searchResults[0].TenSp);
        }

        [Fact]
        public async Task BaoCaoBus_LowStockAndExcelExport_ReturnsExpectedResults()
        {
            using var db = CreateTestDb();
            db.SanPhams.AddRange(
                new SanPham { MaSp = "SP01", TenSp = "Low Stock Phone", MaLoai = "L01", MaNhanHieu = "NH01", MaManHinh = "MH01", GiaNhap = 10_000_000m, GiaBan = 11_000_000m, SoLuong = 2, ThoiGianBaoHanh = 12 },
                new SanPham { MaSp = "SP02", TenSp = "High Stock Phone", MaLoai = "L01", MaNhanHieu = "NH01", MaManHinh = "MH01", GiaNhap = 10_000_000m, GiaBan = 11_000_000m, SoLuong = 25, ThoiGianBaoHanh = 12 }
            );
            db.HoaDonBans.Add(new HoaDonBan { MaHdb = "HDB01", MaNv = "NV01", MaKhach = "KH01", NgayBan = new DateTime(2026, 9, 15), TongTien = 11_000_000m });
            db.ChiTietHdbs.Add(new ChiTietHdb { MaHdb = "HDB01", MaSp = "SP01", SoLuong = 1, KhuyenMai = 0, ThanhTien = 11_000_000m });
            await db.SaveChangesAsync();

            IBaoCaoBUS reportBus = new BaoCaoBUS(db, new ExcelExportService());

            var lowStock = await reportBus.LayCanhBaoTonKhoAdoNetAsync(5);
            Assert.Single(lowStock);
            Assert.Equal("SP01", lowStock[0].MaSp);

            var excelBytes = await reportBus.XuatExcelSpBanChayAsync(9, 2026);
            Assert.NotNull(excelBytes);
            Assert.True(excelBytes.Length > 500);
        }

        [Fact]
        public void AdoNetDataProvider_PrioritizesRetailDbConnectionString()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:RetailDb"] = "Server=preferred;Database=NexusRetailDb;Integrated Security=True;",
                    ["ConnectionStrings:DefaultConnection"] = "Server=fallback;Database=FallbackDb;Integrated Security=True;"
                })
                .Build();

            IAdoNetDataProvider provider = new AdoNetDataProvider(config);
            using var conn = provider.CreateConnection();

            Assert.Contains("preferred", conn.ConnectionString, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task SanPhamBus_InvalidForeignKeys_ReturnsBadRequest()
        {
            using var db = CreateTestDb();
            var bus = new SanPhamBUS(db);

            var badResult = await bus.ThemSanPhamAsync(new ThemSanPhamDto
            {
                TenSp = "Bad FK Phone",
                MaLoai = "L_INVALID",
                MaNhanHieu = "NH01",
                MaManHinh = "MH01",
                GiaNhap = 10_000_000m,
                SoLuong = 5,
                ThoiGianBaoHanh = 12
            });

            Assert.False(badResult.IsSuccess);
            Assert.Equal(400, badResult.StatusCode);
        }

        [Fact]
        public async Task BanHangBus_DuplicateMaSp_MergesLines_And_ConflictingDiscount_ReturnsBadRequest()
        {
            using var db = CreateTestDb();
            db.SanPhams.Add(new SanPham
            {
                MaSp = "SP01", TenSp = "Phone A", MaLoai = "L01", MaNhanHieu = "NH01", MaManHinh = "MH01",
                GiaNhap = 10_000_000m, GiaBan = 12_000_000m, SoLuong = 5, ThoiGianBaoHanh = 12
            });
            await db.SaveChangesAsync();

            var bus = new BanHangBUS(db);

            // Same MaSp with matching discount -> merges to quantity 3
            var merged = await bus.ThanhToanAsync(new HoaDonBanDto
            {
                MaNv = "NV01",
                TenKhachVangLai = "Test",
                DienThoai = "0901234567",
                DanhSachMon = new List<ChiTietHdbDto>
                {
                    new() { MaSp = "SP01", SoLuong = 2, KhuyenMai = 10m },
                    new() { MaSp = "SP01", SoLuong = 1, KhuyenMai = 10m }
                }
            });

            Assert.True(merged.IsSuccess);
            var spAfter = await db.SanPhams.FirstAsync(x => x.MaSp == "SP01");
            Assert.Equal(2, spAfter.SoLuong);

            // Conflicting discount on duplicate MaSp -> 400
            var conflict = await bus.ThanhToanAsync(new HoaDonBanDto
            {
                MaNv = "NV01",
                TenKhachVangLai = "Test",
                DienThoai = "0901234567",
                DanhSachMon = new List<ChiTietHdbDto>
                {
                    new() { MaSp = "SP01", SoLuong = 1, KhuyenMai = 5m },
                    new() { MaSp = "SP01", SoLuong = 1, KhuyenMai = 10m }
                }
            });

            Assert.False(conflict.IsSuccess);
            Assert.Equal(400, conflict.StatusCode);
        }

        [Fact]
        public async Task BanHangController_OnlineAnonymousOrder_StripsMaNvAndKhuyenMai_AndMasksPii()
        {
            using var db = CreateTestDb();
            db.SanPhams.Add(new SanPham
            {
                MaSp = "SP01", TenSp = "Phone X", MaLoai = "L01", MaNhanHieu = "NH01", MaManHinh = "MH01",
                GiaNhap = 10_000_000m, GiaBan = 20_000_000m, SoLuong = 10, ThoiGianBaoHanh = 12
            });
            await db.SaveChangesAsync();

            var controller = new BanHangController(db)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };

            var res = await controller.thanhToan(new HoaDonBanDto
            {
                MaNv = "NV01", // Tampered by anonymous HTTP client -> must be stripped
                TenKhachVangLai = "Nguyễn Văn Khách",
                DienThoai = "0988776655",
                DiaChiGiaoHang = "123 Đường ABC, Hà Nội",
                PhuongThucThanhToan = "VietQR",
                DanhSachMon = new List<ChiTietHdbDto>
                {
                    new() { MaSp = "SP01", SoLuong = 1, KhuyenMai = 50m } // Tampered discount -> must be stripped to 0
                }
            });

            var ok = Assert.IsType<OkObjectResult>(res);
            var body = Assert.IsType<HoaDonBanResponseDto>(ok.Value);
            Assert.Null(body.MaNv);
            Assert.Equal("Khách Online", body.TenNv);
            Assert.Equal(20_000_000m, body.TongTien);

            // Anonymous lookup masks PII
            var lookupRes = await controller.traCuuDonHang("0988776655") as OkObjectResult;
            Assert.NotNull(lookupRes);
            string json = JsonSerializer.Serialize(lookupRes!.Value);
            Assert.Contains("098****655", json);
            Assert.DoesNotContain("Nguyễn Văn Khách", json);
        }

        [Fact]
        public async Task BanHangBus_CancelOrder_RestoresStock_And_ExcludesFromDashboardAndCharts()
        {
            using var db = CreateTestDb();
            db.SanPhams.Add(new SanPham
            {
                MaSp = "SP01", TenSp = "Phone Z", MaLoai = "L01", MaNhanHieu = "NH01", MaManHinh = "MH01",
                GiaNhap = 10_000_000m, GiaBan = 15_000_000m, SoLuong = 5, ThoiGianBaoHanh = 12
            });
            await db.SaveChangesAsync();

            var banBus = new BanHangBUS(db);
            var baoBus = new BaoCaoBUS(db, new ExcelExportService());

            var order = await banBus.ThanhToanAsync(new HoaDonBanDto
            {
                MaNv = "NV01",
                TenKhachVangLai = "Khách Hủy",
                DienThoai = "0909998887",
                DanhSachMon = new List<ChiTietHdbDto> { new() { MaSp = "SP01", SoLuong = 2 } }
            });
            Assert.True(order.IsSuccess);
            Assert.Equal(3, (await db.SanPhams.FirstAsync(x => x.MaSp == "SP01")).SoLuong);

            var cancel = await banBus.CapNhatTrangThaiAsync(order.Data!.MaHdb, new CapNhatTrangThaiDonDto
            {
                TrangThaiDonHang = "DaHuy"
            });
            Assert.True(cancel.IsSuccess);
            Assert.Equal(5, (await db.SanPhams.FirstAsync(x => x.MaSp == "SP01")).SoLuong);

            var charts = await baoBus.LayDuLieuBieuDoDashboardAsync();
            Assert.Equal(6, charts.MonthlyLabels.Count);
            Assert.All(charts.MonthlyRevenuesMillions, rev => Assert.Equal(0m, rev));
        }

        [Fact]
        public async Task CartController_EnforcesStockCap_And_BindsPaymentMethodOnCheckout()
        {
            var httpContext = new DefaultHttpContext
            {
                Session = new InMemoryTestSession()
            };
            var catalog = new StubCatalogClient(new Dictionary<string, SanPhamDto>
            {
                ["SP01"] = new() { MaSp = "SP01", TenSp = "Phone InStock", GiaBan = 15_000_000m, SoLuong = 2 },
                ["SP_OUT"] = new() { MaSp = "SP_OUT", TenSp = "Phone SoldOut", GiaBan = 12_000_000m, SoLuong = 0 }
            });
            var orderClient = new CapturingOrderClient();

            var controller = new CartController(catalog, orderClient, NullLogger<CartController>.Instance)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };

            // Out-of-stock product rejected
            var outRes = await controller.AddToCart("SP_OUT", 1) as JsonResult;
            Assert.NotNull(outRes);
            Assert.False(JsonDocument.Parse(JsonSerializer.Serialize(outRes!.Value)).RootElement.GetProperty("success").GetBoolean());

            // Exceeding available stock rejected
            var okAdd = await controller.AddToCart("SP01", 2) as JsonResult;
            Assert.True(JsonDocument.Parse(JsonSerializer.Serialize(okAdd!.Value)).RootElement.GetProperty("success").GetBoolean());

            var tooMany = await controller.AddToCart("SP01", 1) as JsonResult;
            Assert.False(JsonDocument.Parse(JsonSerializer.Serialize(tooMany!.Value)).RootElement.GetProperty("success").GetBoolean());

            // Checkout binds payment method VietQR & sets MaNv = null
            var checkoutRes = await controller.ProcessCheckout(new CheckoutViewModel
            {
                TenKhach = "Nguyễn Online",
                DienThoai = "0912345678",
                DiaChi = "88 Cầu Giấy, Hà Nội",
                GhiChu = "Giao nhanh",
                PhuongThucThanhToan = "VietQR"
            });

            var redirect = Assert.IsType<RedirectToActionResult>(checkoutRes);
            Assert.Equal("OrderSuccess", redirect.ActionName);
            Assert.NotNull(orderClient.LastOrder);
            Assert.Null(orderClient.LastOrder!.MaNv);
            Assert.Equal("VietQR", orderClient.LastOrder.PhuongThucThanhToan);
            Assert.Equal("88 Cầu Giấy, Hà Nội", orderClient.LastOrder.DiaChiGiaoHang);
            Assert.Equal("Giao nhanh", orderClient.LastOrder.GhiChu);
        }

        private sealed class InMemoryTestSession : ISession
        {
            private readonly Dictionary<string, byte[]> _store = new();
            public bool IsAvailable => true;
            public string Id => "test-session";
            public IEnumerable<string> Keys => _store.Keys;
            public void Clear() => _store.Clear();
            public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
            public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
            public void Remove(string key) => _store.Remove(key);
            public void Set(string key, byte[] value) => _store[key] = value;
            public bool TryGetValue(string key, [NotNullWhen(true)] out byte[]? value) => _store.TryGetValue(key, out value);
        }

        private sealed class StubCatalogClient(Dictionary<string, SanPhamDto> products) : ICatalogApiClient
        {
            public Task<PagedResultDto<SanPhamDto>> GetProductsAsync(int page = 1, int pageSize = 9, string? q = null, string? brand = null, string? price = null)
                => Task.FromResult(new PagedResultDto<SanPhamDto>());
            public Task<PagedResultDto<SanPhamDto>> GetProductsByBrandAsync(string? maNhanHieu, int page = 1, int pageSize = 9)
                => Task.FromResult(new PagedResultDto<SanPhamDto>());
            public Task<PagedResultDto<SanPhamDto>> GetCompareProductsAsync()
                => Task.FromResult(new PagedResultDto<SanPhamDto>());
            public Task<SanPhamDto?> GetProductByIdAsync(string id)
                => Task.FromResult(products.TryGetValue(id, out var sp) ? sp : null);
            public Task<CategoryFilterDto?> GetCategoryFilterAsync()
                => Task.FromResult<CategoryFilterDto?>(null);
        }

        private sealed class CapturingOrderClient : IOrderApiClient
        {
            public HoaDonBanDto? LastOrder { get; private set; }

            public Task<(bool Success, string Message, string? MaHdb, decimal TongTien)> CreateOrderAsync(HoaDonBanDto dto)
            {
                LastOrder = dto;
                return Task.FromResult<(bool, string, string?, decimal)>((true, "OK", "HDB99", 30_000_000m));
            }
            public Task<JsonElement?> GetOrdersByPhoneAsync(string phone) => Task.FromResult<JsonElement?>(null);
            public Task<(List<AdminOrderListItem> Items, int TotalPages, int TotalItems)> GetOrdersListAsync(int page = 1, int pageSize = 15, string? q = null)
                => Task.FromResult((new List<AdminOrderListItem>(), 1, 0));
            public Task<AdminOrderDetailViewModel?> GetOrderDetailAsync(string id) => Task.FromResult<AdminOrderDetailViewModel?>(null);
            public Task<(bool Success, string Message)> UpdateOrderStatusAsync(string maHdb, CapNhatTrangThaiDonDto dto) => Task.FromResult((true, "OK"));
            public Task<(bool IsPaid, decimal PaidAmount, decimal ExpectedAmount, string Message, string? TransactionCode)> GetBankPaymentStatusAsync(string maHdb, decimal? expectedAmount = null)
                => Task.FromResult<(bool, decimal, decimal, string, string?)>((false, 0m, expectedAmount ?? 0m, "Pending", null));
            public Task<(bool Success, string Message)> SimulateBankPaymentAsync(string maHdb, decimal amount) => Task.FromResult((true, "OK"));
        }
    }
}
