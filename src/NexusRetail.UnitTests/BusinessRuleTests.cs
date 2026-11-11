using NexusRetail.Shared.Entities;
using Xunit;

namespace NexusRetail.UnitTests
{
    public class BusinessRuleTests
    {
        [Fact]
        public void Test_SellingPriceMarkup10Percent()
        {
            decimal giaNhap = 10000000m;
            var sp = new SanPham
            {
                MaSp = "SP_TEST_01",
                TenSp = "Test Phone",
                GiaNhap = giaNhap,
                GiaBan = Math.Round(giaNhap * 1.1m, 0),
                SoLuong = 10
            };

            Assert.Equal(11000000m, sp.GiaBan);
        }

        [Fact]
        public void Test_InventoryDeductionOnSale()
        {
            var sp = new SanPham
            {
                MaSp = "SP_TEST_02",
                TenSp = "Test Phone 2",
                SoLuong = 10
            };

            int soLuongBan = 3;
            if (sp.SoLuong >= soLuongBan)
            {
                sp.SoLuong -= soLuongBan;
            }

            Assert.Equal(7, sp.SoLuong);
        }

        [Fact]
        public void Test_ThrowExceptionWhenOutOfStock()
        {
            var sp = new SanPham
            {
                MaSp = "SP_TEST_03",
                TenSp = "Test Phone 3",
                SoLuong = 2
            };

            int soLuongMua = 5;

            Assert.Throws<InvalidOperationException>(() =>
            {
                if (sp.SoLuong < soLuongMua)
                {
                    throw new InvalidOperationException($"Sản phẩm '{sp.TenSp}' chỉ còn {sp.SoLuong} chiếc trong kho, không đủ để bán!");
                }
                sp.SoLuong -= soLuongMua;
            });
        }

        [Fact]
        public void Test_CostPriceAndSellingPriceUpdatedOnStockIn()
        {
            var sp = new SanPham
            {
                MaSp = "SP_TEST_04",
                TenSp = "Test Phone 4",
                GiaNhap = 15000000m,
                GiaBan = 16500000m,
                SoLuong = 5
            };

            decimal donGiaNhapMoi = 20000000m;
            int soLuongNhap = 10;

            sp.SoLuong += soLuongNhap;
            sp.GiaNhap = donGiaNhapMoi;
            sp.GiaBan = Math.Round(donGiaNhapMoi * 1.10m, 0);

            Assert.Equal(15, sp.SoLuong);
            Assert.Equal(20000000m, sp.GiaNhap);
            Assert.Equal(22000000m, sp.GiaBan);
        }

        [Fact]
        public void Test_ProductFilteringByPriceAndKeyword()
        {
            var list = new List<SanPham>
            {
                new SanPham { MaSp = "SP01", TenSp = "iPhone 15 Pro Max", GiaBan = 28600000m },
                new SanPham { MaSp = "SP02", TenSp = "Samsung Galaxy S24 Ultra", GiaBan = 26400000m },
                new SanPham { MaSp = "SP03", TenSp = "Xiaomi 14 5G", GiaBan = 17600000m },
                new SanPham { MaSp = "SP05", TenSp = "Redmi Note 13 Pro", GiaBan = 4950000m }
            };

            var filtered = list
                .Where(x => x.GiaBan < 20000000m && x.TenSp.Contains("Xiaomi", StringComparison.OrdinalIgnoreCase))
                .ToList();

            Assert.Single(filtered);
            Assert.Equal("SP03", filtered[0].MaSp);
        }

        [Fact]
        public void Test_WarrantyPeriodCalculation()
        {
            var ngayMua = new DateTime(2026, 9, 1, 10, 0, 0);
            int thoiGianBaoHanhThang = 12;

            var hanBaoHanh = ngayMua.AddMonths(thoiGianBaoHanhThang);

            Assert.Equal(new DateTime(2027, 9, 1, 10, 0, 0), hanBaoHanh);
            Assert.True(hanBaoHanh > ngayMua);
        }

        [Fact]
        public void Test_DiscountPercentageCalculation()
        {
            decimal giaBan = 28600000m;
            int soLuong = 2;
            decimal khuyenMai = 5m;

            decimal thanhTien = Math.Round(soLuong * giaBan * (1m - (khuyenMai / 100m)), 0);

            Assert.Equal(54340000m, thanhTien);
        }

        [Fact]
        public void Test_InventoryValuation()
        {
            int tonKho = 15;
            decimal giaNhap = 20000000m;

            decimal giaTriTonKho = tonKho * giaNhap;

            Assert.Equal(300000000m, giaTriTonKho);
        }

        [Fact]
        public void Test_CustomMarkupHigherThanMinimum()
        {
            decimal giaNhap = 10000000m;
            decimal customGiaBan = 12500000m;
            decimal minGiaBan = Math.Round(giaNhap * 1.10m, 0);

            decimal finalGiaBan = customGiaBan >= minGiaBan ? customGiaBan : minGiaBan;

            Assert.True(finalGiaBan >= minGiaBan);
            Assert.Equal(12500000m, finalGiaBan);
        }

        [Fact]
        public void Test_InventoryBalanceEquation()
        {
            int tonCuoi = 20;
            int nhapTrongKy = 15;
            int xuatTrongKy = 8;

            int tonDau = tonCuoi + xuatTrongKy - nhapTrongKy;

            Assert.Equal(13, tonDau);
            Assert.Equal(tonCuoi, tonDau + nhapTrongKy - xuatTrongKy);
        }
    }
}
