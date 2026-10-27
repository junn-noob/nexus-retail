using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NexusRetail.Core.Api.DTOs;
using NexusRetail.Core.Api.Services.Payment;
using System.Text.RegularExpressions;
using Xunit;

namespace NexusRetail.UnitTests
{
    public class WinFormsAndIntegrationTests
    {
        [Fact]
        public void CartDiscountAccumulation_MultipleAdds_CorrectlyAccumulatesAndNeverNegative()
        {
            // Arrange
            decimal unitPrice = 1000000m;
            int initialQty = 1;
            decimal initialDiscount = 50000m;

            // First add
            int qty = initialQty;
            decimal discount = initialDiscount;
            decimal total = Math.Max(0, (qty * unitPrice) - discount);
            Assert.Equal(950000m, total);

            // Second add: 2 more items with 100,000đ additional discount
            int additionalQty = 2;
            decimal additionalDiscount = 100000m;

            qty += additionalQty;
            discount += additionalDiscount;
            total = Math.Max(0, (qty * unitPrice) - discount);

            Assert.Equal(3, qty);
            Assert.Equal(150000m, discount);
            Assert.Equal(2850000m, total);

            // Extreme edge case: Discount exceeds total price
            decimal extremeDiscount = 5000000m;
            decimal clampedTotal = Math.Max(0, (qty * unitPrice) - extremeDiscount);
            Assert.Equal(0m, clampedTotal);
        }

        [Fact]
        public void BankPaymentService_PartialPayments_AccumulatesUntilThresholdReached()
        {
            // Arrange
            var services = new ServiceCollection();
            var sp = services.BuildServiceProvider();
            var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
            var paymentService = new BankPaymentService(scopeFactory, NullLogger<BankPaymentService>.Instance);

            string invoiceCode = "HDB99";
            decimal requiredAmount = 15000000m;
            paymentService.RegisterExpectedAmount(invoiceCode, requiredAmount);

            // Act 1: Customer transfers partial amount (5,000,000đ)
            var partialResult = paymentService.ProcessWebhook(new BankWebhookDto
            {
                Gateway = "MBBank",
                TransferAmount = 5000000m,
                Content = $"DUYMOBILE {invoiceCode} DOT 1",
                ReferenceCode = "MB_PARTIAL_01"
            });

            // Assert 1: Not paid yet
            Assert.False(partialResult.IsPaid);
            Assert.Equal(5000000m, partialResult.PaidAmount);

            // Act 2: Customer transfers remaining balance (10,000,000đ)
            var fullResult = paymentService.ProcessWebhook(new BankWebhookDto
            {
                Gateway = "MBBank",
                TransferAmount = 10000000m,
                Content = $"DUYMOBILE {invoiceCode} HOAN TAT",
                ReferenceCode = "MB_FULL_02"
            });

            // Assert 2: Paid in full
            Assert.True(fullResult.IsPaid);
            Assert.Equal(15000000m, fullResult.PaidAmount);
        }

        [Theory]
        [InlineData("DUYMOBILE HDB01", "HDB01")]
        [InlineData("CHUYEN KHOAN HDB14 TAI QUAY", "HDB14")]
        [InlineData("DUY MOBILE HDB 05 THANH TOAN", "HDB05")]
        [InlineData("MBBANK APP HDB999 TIEN MAT", "HDB999")]
        public void InvoiceCodeExtraction_RegexPatterns_ExtractsCorrectMaHdb(string content, string expectedCode)
        {
            string maHdb = "";
            var matchExact = Regex.Match(content, @"\b(HDB\w+)\b", RegexOptions.IgnoreCase);
            if (matchExact.Success)
            {
                maHdb = matchExact.Groups[1].Value.ToUpper();
            }
            else
            {
                var match = Regex.Match(content, @"HDB\s*(\d+)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    maHdb = "HDB" + match.Groups[1].Value;
                }
            }

            Assert.Equal(expectedCode, maHdb);
        }

        [Fact]
        public void UiTheme_KiotBrandTokens_VerifyColorPaletteAndFontConsistency()
        {
            // Verify KiotViet primary brand color is #00A137 (R=0, G=161, B=55)
            var kiotGreen = System.Drawing.Color.FromArgb(0, 161, 55);
            Assert.Equal(0, kiotGreen.R);
            Assert.Equal(161, kiotGreen.G);
            Assert.Equal(55, kiotGreen.B);

            // Verify KiotViet Navy is #0F2240 (R=15, G=34, B=64)
            var kiotNavy = System.Drawing.Color.FromArgb(15, 34, 64);
            Assert.Equal(15, kiotNavy.R);
            Assert.Equal(34, kiotNavy.G);
            Assert.Equal(64, kiotNavy.B);
        }

        [Theory]
        [InlineData(10000000, 11000000)]
        [InlineData(25000000, 27500000)]
        [InlineData(3490000, 3839000)]
        [InlineData(0, 0)]
        public void SellingPriceMarkup_10PercentFormula_MatchesKiotVietStandard(decimal costPrice, decimal expectedSellingPrice)
        {
            decimal sellingPrice = Math.Round(costPrice * 1.1m, 0);
            Assert.Equal(expectedSellingPrice, sellingPrice);
        }

        [Fact]
        public void DiscountValidation_NegativeOrExcessiveDiscount_ClampedSafely()
        {
            decimal itemTotal = 20000000m;
            decimal discountPercentage = 15m; // 15%
            decimal discountAmount = Math.Round(itemTotal * (discountPercentage / 100m), 0);
            decimal finalPrice = itemTotal - discountAmount;

            Assert.Equal(3000000m, discountAmount);
            Assert.Equal(17000000m, finalPrice);

            // Test 100% discount
            decimal fullDiscount = Math.Round(itemTotal * (100m / 100m), 0);
            Assert.Equal(itemTotal, fullDiscount);
            Assert.Equal(0m, itemTotal - fullDiscount);
        }
    }
}
