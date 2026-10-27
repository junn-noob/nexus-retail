using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NexusRetail.Core.Api.DTOs;
using NexusRetail.Core.Api.Services.Payment;
using Xunit;

namespace NexusRetail.UnitTests
{
    public class BankPaymentTests
    {
        private readonly BankPaymentService _paymentService;

        public BankPaymentTests()
        {
            var services = new ServiceCollection();
            var serviceProvider = services.BuildServiceProvider();
            var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _paymentService = new BankPaymentService(scopeFactory, NullLogger<BankPaymentService>.Instance);
        }

        [Fact]
        public void ProcessWebhook_ExactAmountMatch_MarksAsPaid()
        {
            // Arrange
            string maHdb = "HDB01";
            decimal tongTien = 25000000m;
            _paymentService.RegisterExpectedAmount(maHdb, tongTien);

            var webhookDto = new BankWebhookDto
            {
                Gateway = "MBBank",
                TransferAmount = tongTien,
                Content = "DUYMOBILE HDB01 THANH TOAN",
                ReferenceCode = "FT261027123456"
            };

            // Act
            var result = _paymentService.ProcessWebhook(webhookDto);

            // Assert
            Assert.True(result.IsPaid);
            Assert.Equal(tongTien, result.PaidAmount);
            Assert.Equal(maHdb, result.MaHdb);
            Assert.Contains("thành công", result.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ProcessWebhook_Underpayment_DetectsMissingAmountAndWarns()
        {
            // Arrange
            string maHdb = "HDB02";
            decimal tongTien = 30000000m;
            decimal daChuyen = 20000000m; // Thiếu 10 triệu
            _paymentService.RegisterExpectedAmount(maHdb, tongTien);

            var webhookDto = new BankWebhookDto
            {
                Gateway = "MBBank",
                TransferAmount = daChuyen,
                Content = "DUYMOBILE HDB02 CHUYEN TIEN",
                ReferenceCode = "FT261027999999"
            };

            // Act
            var result = _paymentService.ProcessWebhook(webhookDto);

            // Assert
            Assert.False(result.IsPaid);
            Assert.Equal(daChuyen, result.PaidAmount);
            Assert.Contains("thiếu", result.Message, StringComparison.OrdinalIgnoreCase);
            Assert.Contains((tongTien - daChuyen).ToString("N0"), result.Message);
        }

        [Fact]
        public void ProcessWebhook_AccumulatedPartialPayments_SucceedsWhenTotalReached()
        {
            // Arrange: Khách nộp tiền làm 2 đợt (Đợt 1: 10tr, Đợt 2: 5tr cho đơn 15tr)
            string maHdb = "HDB03";
            decimal tongTien = 15000000m;
            _paymentService.RegisterExpectedAmount(maHdb, tongTien);

            // Act 1: Chuyển đợt 1
            var dot1 = _paymentService.ProcessWebhook(new BankWebhookDto
            {
                Gateway = "MBBank",
                TransferAmount = 10000000m,
                Content = "HDB03 DOT 1",
                ReferenceCode = "FT_PART_1"
            });

            // Assert 1: Chưa đủ tiền
            Assert.False(dot1.IsPaid);
            Assert.Equal(10000000m, dot1.PaidAmount);

            // Act 2: Chuyển tiếp đợt 2 đủ 5tr
            var dot2 = _paymentService.ProcessWebhook(new BankWebhookDto
            {
                Gateway = "MBBank",
                TransferAmount = 5000000m,
                Content = "HDB03 DOT 2 NOT TIEN",
                ReferenceCode = "FT_PART_2"
            });

            // Assert 2: Lũy kế đủ 15tr -> Hoàn tất thanh toán
            Assert.True(dot2.IsPaid);
            Assert.Equal(15000000m, dot2.PaidAmount);
            Assert.Contains("thành công", dot2.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("DUYMOBILE HDB05", "HDB05")]
        [InlineData("HDB12 CHUYEN TIEN POS", "HDB12")]
        [InlineData("DUY MOBILE HDB 08 CHUYEN KHOAN", "HDB08")]
        [InlineData("KHACH HANG DUYMOBILE HDB999 NAPAS", "HDB999")]
        public void ProcessWebhook_RegexVariations_ExtractsCorrectMaHdb(string transferContent, string expectedMaHdb)
        {
            // Arrange
            _paymentService.RegisterExpectedAmount(expectedMaHdb, 5000000m);

            var dto = new BankWebhookDto
            {
                Gateway = "MBBank",
                TransferAmount = 5000000m,
                Content = transferContent,
                ReferenceCode = "FT_VAR"
            };

            // Act
            var result = _paymentService.ProcessWebhook(dto);

            // Assert
            Assert.True(result.IsPaid);
            Assert.Equal(expectedMaHdb, result.MaHdb);
        }

        [Fact]
        public void ProcessWebhook_Overpayment_MarksAsPaid()
        {
            // Arrange: Đơn 10tr nhưng khách chuyển 12tr (thừa tiền)
            string maHdb = "HDB06";
            decimal tongTien = 10000000m;
            decimal thucChuyen = 12000000m;
            _paymentService.RegisterExpectedAmount(maHdb, tongTien);

            // Act
            var result = _paymentService.ProcessWebhook(new BankWebhookDto
            {
                Gateway = "MBBank",
                TransferAmount = thucChuyen,
                Content = "DUYMOBILE HDB06 THUA TIEN",
                ReferenceCode = "FT_OVER"
            });

            // Assert
            Assert.True(result.IsPaid);
            Assert.Equal(thucChuyen, result.PaidAmount);
            Assert.Contains("thành công", result.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void GetPaymentStatus_UnpaidOrder_ReturnsPending()
        {
            // Act
            var status = _paymentService.GetPaymentStatus("HDB_CHUA_CO", 10000000m);

            // Assert
            Assert.False(status.IsPaid);
            Assert.Equal(0, status.PaidAmount);
            Assert.Contains("Đang chờ thanh toán", status.Message);
        }
    }
}
