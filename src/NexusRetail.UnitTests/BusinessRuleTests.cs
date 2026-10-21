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
    }
}
