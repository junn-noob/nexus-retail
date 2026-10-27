using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NexusRetail.Core.Api.Controllers;
using NexusRetail.Core.Api.Data;
using NexusRetail.Shared.DTOs.BanHang;
using NexusRetail.Shared.DTOs.NhapHang;
using NexusRetail.Shared.DTOs.SanPham;
using NexusRetail.Shared.Entities;
using Xunit;

namespace NexusRetail.UnitTests
{
    public class RetailWorkflowTests
    {
        private RetailDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<RetailDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            var db = new RetailDbContext(options);

            // Seed reference data (NhanVien, NhaCungCap, KhachHang, Loai, NhanHieu, ManHinh)
            db.NhanViens.Add(new NhanVien
            {
                MaNv = "NV01",
                TenNv = "Nguyễn Văn Thu Ngân",
                GioiTinh = "Nam",
                DienThoai = "0988111222"
            });

            db.NhaCungCaps.Add(new NhaCungCap
            {
                MaNcc = "NCC01",
                TenNcc = "Công Ty TNHH Apple Việt Nam",
                DienThoai = "0243999888"
            });

            db.KhachHangs.Add(new KhachHang
            {
                MaKhach = "KH01",
                TenKhach = "Trần Thị Khách Hàng",
                DienThoai = "0901234567",
                DiaChi = "Hà Nội"
            });

            db.Loais.Add(new Loai { MaLoai = "L01", TenLoai = "Smartphone" });
            db.NhanHieus.Add(new NhanHieu { MaNhanHieu = "NH01", TenNhanHieu = "Apple" });
            db.ManHinhs.Add(new ManHinh { MaManHinh = "MH01", TenManHinh = "OLED 6.7 inch" });

            // Seed test products
            db.SanPhams.Add(new SanPham
            {
                MaSp = "SP01",
                TenSp = "iPhone 16 Pro Max 256GB",
                MaLoai = "L01",
                MaNhanHieu = "NH01",
                MaManHinh = "MH01",
                GiaNhap = 28000000m,
                GiaBan = 30800000m,
                SoLuong = 10,
                ThoiGianBaoHanh = 12
            });

            db.SanPhams.Add(new SanPham
            {
                MaSp = "SP02",
                TenSp = "iPhone 15 128GB",
                MaLoai = "L01",
                MaNhanHieu = "NH01",
                MaManHinh = "MH01",
                GiaNhap = 18000000m,
                GiaBan = 19800000m,
                SoLuong = 2,
                ThoiGianBaoHanh = 12
            });

            db.SaveChanges();
            return db;
        }

        [Fact]
        public async Task BanHang_Checkout_DecrementsStock_And_GeneratesInvoice()
        {
            // Arrange
            using var db = CreateInMemoryDbContext();
            var controller = new BanHangController(db);

            var dto = new HoaDonBanDto
            {
                MaHdb = "HDB_TEST_01",
                TenKhachVangLai = "Trần Thị Khách Hàng",
                DienThoai = "0901234567",
                DanhSachMon = new List<ChiTietHdbDto>
                {
                    new ChiTietHdbDto
                    {
                        MaSp = "SP01",
                        SoLuong = 3,
                        KhuyenMai = 0
                    }
                }
            };

            // Act
            var result = await controller.thanhToan(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<HoaDonBanResponseDto>(okResult.Value);
            Assert.Equal("HDB_TEST_01", response.MaHdb);

            // Verify stock reduction in database (10 - 3 = 7)
            var sp = await db.SanPhams.FirstAsync(x => x.MaSp == "SP01");
            Assert.Equal(7, sp.SoLuong);

            // Verify invoice and details saved
            var hdb = await db.HoaDonBans.Include(h => h.ChiTietHdbs).FirstOrDefaultAsync(h => h.MaHdb == "HDB_TEST_01");
            Assert.NotNull(hdb);
            Assert.Single(hdb.ChiTietHdbs);
            Assert.Equal(3 * 30800000m, hdb.TongTien);
        }

        [Fact]
        public async Task BanHang_Checkout_OutOfStock_FailsGracefully_WithoutDeduction()
        {
            // Arrange
            using var db = CreateInMemoryDbContext();
            var controller = new BanHangController(db);

            // SP02 only has 2 items in stock; request 5 items
            var dto = new HoaDonBanDto
            {
                MaHdb = "HDB_TEST_FAIL",
                TenKhachVangLai = "Khách Test",
                DienThoai = "0911222333",
                DanhSachMon = new List<ChiTietHdbDto>
                {
                    new ChiTietHdbDto
                    {
                        MaSp = "SP02",
                        SoLuong = 5,
                        KhuyenMai = 0
                    }
                }
            };

            // Act
            var result = await controller.thanhToan(dto);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult.Value);

            // Verify stock remains untouched (still 2)
            var sp = await db.SanPhams.FirstAsync(x => x.MaSp == "SP02");
            Assert.Equal(2, sp.SoLuong);

            // Verify invoice was not saved
            var hdb = await db.HoaDonBans.FirstOrDefaultAsync(h => h.MaHdb == "HDB_TEST_FAIL");
            Assert.Null(hdb);
        }

        [Fact]
        public async Task NhapHang_CreatesReceipt_IncrementsStock_And_RecalculatesGiaBan110Percent()
        {
            // Arrange
            using var db = CreateInMemoryDbContext();
            var controller = new NhapHangController(db);

            // Initial: SP01 has SoLuong = 10, GiaNhap = 28,000,000, GiaBan = 30,800,000
            // Inbound shipment: 5 items @ 25,000,000 VND
            var dto = new HoaDonNhapDto
            {
                MaNv = "NV01",
                MaNcc = "NCC01",
                DanhSachMon = new List<ChiTietHdnDto>
                {
                    new ChiTietHdnDto
                    {
                        MaSp = "SP01",
                        SoLuong = 5,
                        DonGia = 25000000m,
                        KhuyenMai = 0
                    }
                }
            };

            // Act
            var result = await controller.nhapHang(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var response = Assert.IsType<HoaDonNhapResponseDto>(createdResult.Value);
            Assert.NotEmpty(response.MaHdn);

            // Verify inventory increment: 10 + 5 = 15
            var sp = await db.SanPhams.FirstAsync(x => x.MaSp == "SP01");
            Assert.Equal(15, sp.SoLuong);

            // Verify BR-02 (GiaNhap updated to 25,000,000)
            Assert.Equal(25000000m, sp.GiaNhap);

            // Verify BR-03 (GiaBan updated to 110% of GiaNhap = 27,500,000)
            Assert.Equal(27500000m, sp.GiaBan);
        }

        [Fact]
        public async Task SanPham_ThemSanPham_CalculatesGiaBanAutomatically()
        {
            // Arrange
            using var db = CreateInMemoryDbContext();
            var controller = new SanPhamController(db);

            var dto = new ThemSanPhamDto
            {
                TenSp = "Xiaomi 15 Ultra",
                MaLoai = "L01",
                MaNhanHieu = "NH01",
                MaManHinh = "MH01",
                GiaNhap = 20000000m, // 20 million
                SoLuong = 8,
                ThoiGianBaoHanh = 18,
                GhiChu = "Flagship Snapdragon 8 Elite"
            };

            // Act
            var result = await controller.themSanPham(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var sp = await db.SanPhams.FirstOrDefaultAsync(x => x.TenSp == "Xiaomi 15 Ultra");
            Assert.NotNull(sp);
            Assert.Equal(20000000m, sp.GiaNhap);
            // 20M * 1.10 = 22M
            Assert.Equal(22000000m, sp.GiaBan);
        }

        [Fact]
        public async Task SanPham_XoaSanPham_WithHistoricalInvoices_ReturnsBadRequest()
        {
            // Arrange
            using var db = CreateInMemoryDbContext();
            // Create an invoice referencing SP01
            db.HoaDonBans.Add(new HoaDonBan
            {
                MaHdb = "HDB99",
                MaNv = "NV01",
                NgayBan = DateTime.Now,
                MaKhach = "KH01",
                TongTien = 30800000m
            });
            db.ChiTietHdbs.Add(new ChiTietHdb
            {
                MaHdb = "HDB99",
                MaSp = "SP01",
                SoLuong = 1,
                KhuyenMai = 0,
                ThanhTien = 30800000m
            });
            await db.SaveChangesAsync();

            var controller = new SanPhamController(db);

            // Act: Attempt to delete SP01 which has invoice history
            var result = await controller.xoaSanPham("SP01");

            // Assert: Must be rejected with BadRequest
            var badResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badResult.Value);

            // Ensure product is still present
            var sp = await db.SanPhams.FindAsync("SP01");
            Assert.NotNull(sp);
        }

        [Fact]
        public async Task SanPham_XoaSanPham_Unreferenced_SucceedsWith204()
        {
            // Arrange
            using var db = CreateInMemoryDbContext();
            // SP02 has no invoice history
            var controller = new SanPhamController(db);

            // Act
            var result = await controller.xoaSanPham("SP02");

            // Assert
            Assert.IsType<NoContentResult>(result);
            var sp = await db.SanPhams.FindAsync("SP02");
            Assert.Null(sp);
        }
    }
}
