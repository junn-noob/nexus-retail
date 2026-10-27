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
    }
}
