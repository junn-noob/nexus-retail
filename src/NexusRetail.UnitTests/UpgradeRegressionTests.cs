using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using NexusRetail.Core.Api.BUS;
using NexusRetail.Core.Api.Controllers;
using NexusRetail.Core.Api.Data;
using NexusRetail.Core.Api.Services.Export;
using NexusRetail.Shared.DTOs.BanHang;
using NexusRetail.Shared.DTOs.SanPham;
using NexusRetail.Shared.Entities;
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
    }
}
