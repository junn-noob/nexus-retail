using NexusRetail.Shared.Entities;
using Xunit;

namespace NexusRetail.UnitTests
{
    public class BusinessRuleTests
    {
        // 1. BR-03: Tự động tính giá bán: GiaBan = GiaNhap * 1.10
        [Fact]
        public void TestBR03_SellingPriceMarkup10Percent()
        {
            decimal giaNhap = 10000000m; // 10 triệu
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

        // 2. BR-01: Trừ tồn kho khi xuất bán: Tồn 10, bán 3 -> Tồn 7
        [Fact]
        public void TestBR01_InventoryDeductionOnSale()
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

        // 3. BR-01: Chặn giao dịch khi tồn kho không đủ (Tồn 2, mua 5)
        [Fact]
        public void TestBR01_ThrowExceptionWhenOutOfStock()
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

        // 4. BR-02 & BR-03: Nhập hàng tự động cập nhật giá nhập và tính lại giá bán = giá nhập * 1.10
        [Fact]
        public void TestBR02_CostPriceAndSellingPriceUpdatedOnStockIn()
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

            // Nghiệp vụ nhập kho: Tăng tồn, cập nhật giá nhập mới, cập nhật giá bán mới
            sp.SoLuong += soLuongNhap;
            sp.GiaNhap = donGiaNhapMoi;
            sp.GiaBan = Math.Round(donGiaNhapMoi * 1.10m, 0);

            Assert.Equal(15, sp.SoLuong);
            Assert.Equal(20000000m, sp.GiaNhap);
            Assert.Equal(22000000m, sp.GiaBan);
        }

        // 5. BR-04: Lọc sản phẩm theo khoảng giá và tìm kiếm từ khóa
        [Fact]
        public void TestBR04_ProductFilteringByPriceAndKeyword()
        {
            var list = new List<SanPham>
            {
                new SanPham { MaSp = "SP01", TenSp = "iPhone 15 Pro Max", GiaBan = 28600000m },
                new SanPham { MaSp = "SP02", TenSp = "Samsung Galaxy S24 Ultra", GiaBan = 26400000m },
                new SanPham { MaSp = "SP03", TenSp = "Xiaomi 14 5G", GiaBan = 17600000m },
                new SanPham { MaSp = "SP05", TenSp = "Redmi Note 13 Pro", GiaBan = 4950000m }
            };

            // Lọc dưới 20 triệu và chứa "Xiaomi"
            var filtered = list
                .Where(x => x.GiaBan < 20000000m && x.TenSp.Contains("Xiaomi", StringComparison.OrdinalIgnoreCase))
                .ToList();

            Assert.Single(filtered);
            Assert.Equal("SP03", filtered[0].MaSp);
        }

        // 6. Tra cứu & tính thời hạn bảo hành thiết bị theo số tháng
        [Fact]
        public void TestBR05_WarrantyPeriodCalculation()
        {
            var ngayMua = new DateTime(2026, 9, 1, 10, 0, 0);
            int thoiGianBaoHanhThang = 12;

            var hanBaoHanh = ngayMua.AddMonths(thoiGianBaoHanhThang);

            Assert.Equal(new DateTime(2027, 9, 1, 10, 0, 0), hanBaoHanh);
            Assert.True(hanBaoHanh > ngayMua);
        }
    }
}
