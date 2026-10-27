using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NexusRetail.Core.Api.Controllers;
using NexusRetail.Core.Api.DTOs;
using NexusRetail.Core.Api.Services.Payment;
using Xunit;

namespace NexusRetail.UnitTests
{
    public class CoreControllerTests
    {
        private readonly WebhookController _webhookController;
        private readonly BankPaymentService _paymentService;

        public CoreControllerTests()
        {
            var services = new ServiceCollection();
            var serviceProvider = services.BuildServiceProvider();
            var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

            _paymentService = new BankPaymentService(scopeFactory, NullLogger<BankPaymentService>.Instance);
            _webhookController = new WebhookController(_paymentService, NullLogger<WebhookController>.Instance);
        }

        [Fact]
        public void ReceiveBankWebhook_InvalidAmount_ReturnsBadRequest()
        {
            // Arrange
            var dto = new BankWebhookDto
            {
                Gateway = "MBBank",
                TransferAmount = -5000,
                Content = "DUYMOBILE HDB01"
            };

            // Act
            var result = _webhookController.ReceiveBankWebhook(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        private static object? GetProp(object? obj, string propName)
        {
            return obj?.GetType().GetProperty(propName)?.GetValue(obj);
        }

        [Fact]
        public void ReceiveBankWebhook_ValidWebhook_UnderpaymentDetected()
        {
            // Arrange: Đơn hàng cần 25 triệu, khách chuyển 15 triệu
            string maHdb = "HDB01";
            decimal expected = 25000000m;
            decimal transferred = 15000000m;
            _paymentService.RegisterExpectedAmount(maHdb, expected);

            var dto = new BankWebhookDto
            {
                Gateway = "MBBank",
                TransferAmount = transferred,
                Content = $"DUYMOBILE {maHdb} CHUYEN KHOAN",
                ReferenceCode = "FT_WEBHOOK_01"
            };

            // Act: 1. Bắn webhook chuyển thiếu
            var actionResult = _webhookController.ReceiveBankWebhook(dto);
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var success = (bool?)GetProp(okResult.Value, "success");
            Assert.False(success);

            // Act: 2. Kiểm tra trạng thái đơn qua endpoint GET /api/webhook/bank/status/{maHdb}
            var statusResult = _webhookController.CheckPaymentStatus(maHdb, expected);
            var okStatus = Assert.IsType<OkObjectResult>(statusResult);
            var paymentStatus = Assert.IsType<PaymentStatusDto>(okStatus.Value);

            // Assert
            Assert.False(paymentStatus.IsPaid);
            Assert.Equal(transferred, paymentStatus.PaidAmount);
            Assert.Contains("thiếu", paymentStatus.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ReceiveBankWebhook_FullPayment_ConfirmedSuccessfully()
        {
            // Arrange: Đơn hàng cần 28.6 triệu, khách chuyển đúng 28.6 triệu
            string maHdb = "HDB07";
            decimal expected = 28600000m;
            _paymentService.RegisterExpectedAmount(maHdb, expected);

            var dto = new BankWebhookDto
            {
                Gateway = "MBBank",
                TransferAmount = expected,
                Content = $"DUYMOBILE {maHdb} THANH TOAN DU",
                ReferenceCode = "FT_WEBHOOK_FULL"
            };

            // Act: Bắn webhook
            var actionResult = _webhookController.ReceiveBankWebhook(dto);
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var success = (bool?)GetProp(okResult.Value, "success");
            Assert.True(success);

            // Kiểm tra trạng thái
            var statusResult = _webhookController.CheckPaymentStatus(maHdb, expected);
            var okStatus = Assert.IsType<OkObjectResult>(statusResult);
            var paymentStatus = Assert.IsType<PaymentStatusDto>(okStatus.Value);

            // Assert
            Assert.True(paymentStatus.IsPaid);
            Assert.Equal(expected, paymentStatus.PaidAmount);
            Assert.Equal("FT_WEBHOOK_FULL", paymentStatus.TransactionCode);
        }

        [Fact]
        public void SimulateWebhookPayment_ValidRequest_RegistersAndReturnsOk()
        {
            // Arrange
            var req = new SimulatePaymentRequest
            {
                MaHdb = "HDB08",
                Amount = 28600000m
            };

            // Act
            var result = _webhookController.SimulateWebhookPayment(req);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var success = (bool?)GetProp(okResult.Value, "success");
            var message = (string?)GetProp(okResult.Value, "message");
            Assert.True(success);
            Assert.Contains("HDB08", message);
        }

        [Fact]
        public void GetWebhookHistory_AfterTransactions_ContainsRecords()
        {
            // Arrange
            _webhookController.ReceiveBankWebhook(new BankWebhookDto
            {
                Gateway = "MBBank",
                TransferAmount = 1000000m,
                Content = "DUYMOBILE HDB09 TEST",
                ReferenceCode = "HIST_01"
            });

            // Act
            var result = _webhookController.GetWebhookHistory();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var history = Assert.IsAssignableFrom<IEnumerable<BankWebhookDto>>(okResult.Value);
            Assert.NotEmpty(history);
        }
    }
}
