using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using NexusRetail.Core.Api.BUS;
using NexusRetail.Core.Api.Data;
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
    public class ColorVariantTests
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
            db.ManHinhs.Add(new ManHinh { MaManHinh = "MH01", TenManHinh = "OLED 6.9 inch" });
            db.NhanViens.Add(new NhanVien { MaNv = "NV01", TenNv = "Nguyễn Văn Hùng", GioiTinh = "Nam", DienThoai = "0900111222" });
            db.KhachHangs.Add(new KhachHang { MaKhach = "KH01", TenKhach = "Trần Thị Lan", DienThoai = "0911222333" });

            // Seed color catalog
            db.MauSacs.AddRange(
                new MauSac { MaMau = "TITAN_NATURAL", TenMau = "Titan Tự Nhiên", MaHex = "#9a948d" },
                new MauSac { MaMau = "TITAN_BLACK", TenMau = "Titan Đen", MaHex = "#3c3b37" },
                new MauSac { MaMau = "TITAN_DESERT", TenMau = "Titan Sa Mạc", MaHex = "#c5b39b" }
            );

            // Product: iPhone 16 Pro Max with 3 color variants
            var sp = new SanPham
            {
                MaSp = "SP01",
                TenSp = "iPhone 16 Pro Max 256GB",
                MaLoai = "L01",
                MaNhanHieu = "NH01",
                MaManHinh = "MH01",
                GiaNhap = 28_000_000m,
                GiaBan = 34_990_000m,
                SoLuong = 30,
                ThoiGianBaoHanh = 12
            };
            db.SanPhams.Add(sp);

            // Color variants: different stock and prices
            db.SanPhamMauSacs.AddRange(
                new SanPhamMauSac
                {
                    MaSp = "SP01",
                    MaMau = "TITAN_NATURAL",
                    GiaNhap = 29_000_000m,
                    GiaBan = 35_990_000m,
                    SoLuong = 15,
                    HinhAnh = "iphone_16_pm_natural.png"
                },
                new SanPhamMauSac
                {
                    MaSp = "SP01",
                    MaMau = "TITAN_BLACK",
                    GiaNhap = 28_000_000m,
                    GiaBan = 34_990_000m,
                    SoLuong = 10,
                    HinhAnh = "iphone_16_pm_black.png"
                },
                new SanPhamMauSac
                {
                    MaSp = "SP01",
                    MaMau = "TITAN_DESERT",
                    GiaNhap = 29_500_000m,
                    GiaBan = 36_490_000m,
                    SoLuong = 5,
                    HinhAnh = "iphone_16_pm_desert.png"
                }
            );

            db.SaveChanges();
            return db;
        }

        [Fact]
        public async Task SanPhamBUS_ChiTietSanPhamAsync_CalculatesMinMaxPriceAndReturnsVariants()
        {
            using var db = CreateTestDb();
            var bus = new SanPhamBUS(db);

            var spDto = await bus.ChiTietSanPhamAsync("SP01");

            Assert.NotNull(spDto);
            Assert.Equal("SP01", spDto.MaSp);
            Assert.Equal(3, spDto.DanhSachMauSac.Count);
            // Min price should be Titan Đen (34,990,000), Max price should be Titan Sa Mạc (36,490,000)
            Assert.Equal(34_990_000m, spDto.GiaBanThapNhat);
            Assert.Equal(36_490_000m, spDto.GiaBanCaoNhat);
            // Verify variant details
            var naturalVariant = spDto.DanhSachMauSac.FirstOrDefault(v => v.MaMau == "TITAN_NATURAL");
            Assert.NotNull(naturalVariant);
            Assert.Equal(35_990_000m, naturalVariant.GiaBan);
            Assert.Equal("Titan Tự Nhiên", naturalVariant.TenMau);
        }

        [Fact]
        public async Task BanHangBUS_CreateSaleOrder_DeductsVariantStockAndUsesVariantPrice()
        {
            using var db = CreateTestDb();
            var banHangBus = new BanHangBUS(db, NullLogger<BanHangBUS>.Instance);

            // Purchase 2 units of Titan Sa Mạc (highest priced: 36,490,000, initial stock = 5)
            var orderDto = new HoaDonBanDto
            {
                MaNv = "NV01",
                MaKhach = "KH01",
                PhuongThucThanhToan = "Cash",
                DanhSachMon = new List<ChiTietHdbDto>
                {
                    new()
                    {
                        MaSp = "SP01",
                        MaMau = "TITAN_DESERT",
                        SoLuong = 2,
                        KhuyenMai = 0
                    }
                }
            };

            var result = await banHangBus.ThanhToanAsync(orderDto);

            Assert.True(result.IsSuccess, result.ErrorMessage);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.MaHdb);
            // Total should be 2 * 36,490,000 = 72,980,000
            Assert.Equal(72_980_000m, result.Data.TongTien);

            // Check variant stock deduction: was 5, bought 2 -> should be 3
            var desertVariant = await db.SanPhamMauSacs.FirstOrDefaultAsync(v => v.MaSp == "SP01" && v.MaMau == "TITAN_DESERT");
            Assert.NotNull(desertVariant);
            Assert.Equal(3, desertVariant.SoLuong);

            // Parent product total stock should be 30 - 2 = 28
            var sp = await db.SanPhams.FindAsync("SP01");
            Assert.NotNull(sp);
            Assert.Equal(28, sp.SoLuong);
        }

        [Fact]
        public async Task BanHangBUS_CreateSaleOrder_FailsWhenVariantStockInsufficient()
        {
            using var db = CreateTestDb();
            var banHangBus = new BanHangBUS(db, NullLogger<BanHangBUS>.Instance);

            // Attempt to buy 10 units of Titan Sa Mạc (stock is only 5)
            var orderDto = new HoaDonBanDto
            {
                MaNv = "NV01",
                MaKhach = "KH01",
                PhuongThucThanhToan = "Cash",
                DanhSachMon = new List<ChiTietHdbDto>
                {
                    new()
                    {
                        MaSp = "SP01",
                        MaMau = "TITAN_DESERT",
                        SoLuong = 10,
                        KhuyenMai = 0
                    }
                }
            };

            var result = await banHangBus.ThanhToanAsync(orderDto);

            Assert.False(result.IsSuccess);
            Assert.Contains("không đủ để bán", result.ErrorMessage);
        }

        [Fact]
        public void CartItem_CartKey_DistinguishesMultipleColorsOfSameModel()
        {
            var cartItem1 = new CartItem
            {
                MaSp = "SP01",
                TenSp = "iPhone 16 Pro Max",
                MaMau = "TITAN_NATURAL",
                TenMau = "Titan Tự Nhiên",
                GiaBan = 35_990_000m,
                SoLuong = 1
            };

            var cartItem2 = new CartItem
            {
                MaSp = "SP01",
                TenSp = "iPhone 16 Pro Max",
                MaMau = "TITAN_BLACK",
                TenMau = "Titan Đen",
                GiaBan = 34_990_000m,
                SoLuong = 2
            };

            // Keys must be distinct
            Assert.Equal("SP01_TITAN_NATURAL", cartItem1.CartKey);
            Assert.Equal("SP01_TITAN_BLACK", cartItem2.CartKey);
            Assert.NotEqual(cartItem1.CartKey, cartItem2.CartKey);

            // In a list/cart, both items can coexist
            var cart = new List<CartItem> { cartItem1, cartItem2 };
            Assert.Equal(2, cart.Count);
            Assert.Equal(35_990_000m + (34_990_000m * 2), cart.Sum(c => c.ThanhTien));
        }

        [Fact]
        public void CartController_RemoveItemByCartKey_RemovesOnlyTargetColorVariant()
        {
            var initialCart = new List<CartItem>
            {
                new() { MaSp = "SP01", MaMau = "TITAN_NATURAL", TenMau = "Titan Tự Nhiên", GiaBan = 35_990_000m, SoLuong = 1 },
                new() { MaSp = "SP01", MaMau = "TITAN_BLACK", TenMau = "Titan Đen", GiaBan = 34_990_000m, SoLuong = 1 }
            };

            var session = new TestSession();
            session.Set("NEXUS_CART_SESSION", System.Text.Encoding.UTF8.GetBytes(JsonSerializer.Serialize(initialCart)));

            var catalogClient = new TestCatalogClient();
            var orderClient = new TestOrderClient();
            var controller = new CartController(catalogClient, orderClient, NullLogger<CartController>.Instance);

            var httpContext = new DefaultHttpContext { Session = session };
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
            controller.TempData = new TempDataDictionary(httpContext, new TestTempDataProvider());

            // Remove only TITAN_BLACK
            var result = controller.RemoveItem(cartKey: "SP01_TITAN_BLACK", maSp: "SP01") as RedirectToActionResult;
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);

            // Re-read cart from session
            session.TryGetValue("NEXUS_CART_SESSION", out var updatedBytes);
            Assert.NotNull(updatedBytes);
            var updatedCart = JsonSerializer.Deserialize<List<CartItem>>(System.Text.Encoding.UTF8.GetString(updatedBytes))!;
            Assert.Single(updatedCart);
            Assert.Equal("TITAN_NATURAL", updatedCart[0].MaMau);
        }

        private sealed class TestSession : ISession
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

        private sealed class TestTempDataProvider : ITempDataProvider
        {
            public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
            public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
        }

        private sealed class TestCatalogClient : ICatalogApiClient
        {
            public Task<PagedResultDto<SanPhamDto>> GetProductsAsync(int page = 1, int pageSize = 9, string? q = null, string? brand = null, string? price = null)
                => Task.FromResult(new PagedResultDto<SanPhamDto>());
            public Task<PagedResultDto<SanPhamDto>> GetProductsByBrandAsync(string? maNhanHieu, int page = 1, int pageSize = 9)
                => Task.FromResult(new PagedResultDto<SanPhamDto>());
            public Task<PagedResultDto<SanPhamDto>> GetCompareProductsAsync()
                => Task.FromResult(new PagedResultDto<SanPhamDto>());
            public Task<SanPhamDto?> GetProductByIdAsync(string id)
                => Task.FromResult<SanPhamDto?>(null);
            public Task<CategoryFilterDto?> GetCategoryFilterAsync()
                => Task.FromResult<CategoryFilterDto?>(null);
        }

        private sealed class TestOrderClient : IOrderApiClient
        {
            public Task<(bool Success, string Message, string? MaHdb, decimal TongTien)> CreateOrderAsync(HoaDonBanDto dto)
                => Task.FromResult<(bool, string, string?, decimal)>((true, "OK", "HDB99", 30_000_000m));
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
