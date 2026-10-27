using Microsoft.AspNetCore.Mvc;
using NexusRetail.Core.Api.DTOs;
using NexusRetail.Core.Api.Services.Payment;

namespace NexusRetail.Core.Api.Controllers
{
    [ApiController]
    [Route("api/webhook")]
    public class WebhookController : ControllerBase
    {
        private readonly IBankPaymentService _paymentService;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(IBankPaymentService paymentService, ILogger<WebhookController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint đón Webhook biến động số dư từ script bot MBBank (hoặc SePay / Casso / PayOS)
        /// </summary>
        [HttpPost("bank")]
        public IActionResult ReceiveBankWebhook([FromBody] BankWebhookDto dto)
        {
            if (dto == null || dto.TransferAmount <= 0)
            {
                return BadRequest(new { success = false, message = "Dữ liệu webhook không hợp lệ hoặc số tiền <= 0" });
            }

            var result = _paymentService.ProcessWebhook(dto);
            return Ok(new
            {
                success = result.IsPaid,
                data = result
            });
        }

        /// <summary>
        /// Endpoint cho WinForms POS hoặc Web kiểm tra trạng thái thanh toán của hóa đơn (có thể truyền expectedAmount để so khớp số tiền)
        /// </summary>
        [HttpGet("bank/status/{maHdb}")]
        public IActionResult CheckPaymentStatus(string maHdb, [FromQuery] decimal? expectedAmount = null)
        {
            var status = _paymentService.GetPaymentStatus(maHdb, expectedAmount);
            return Ok(status);
        }

        /// <summary>
        /// Endpoint Sandbox / Mock kiểm thử tích hợp cổng ngân hàng (Hỗ trợ Automated Testing & CI/CD)
        /// </summary>
        [HttpPost("bank/simulate")]
        public IActionResult SimulateWebhookPayment([FromBody] SimulatePaymentRequest req)
        {
            if (string.IsNullOrWhiteSpace(req?.MaHdb))
            {
                return BadRequest(new { success = false, message = "Vui lòng cung cấp mã hóa đơn MaHdb" });
            }

            var result = _paymentService.SimulatePayment(req.MaHdb, req.Amount > 0 ? req.Amount : 10000000m);
            return Ok(new
            {
                success = true,
                message = $"Ghi nhận giao dịch Sandbox MBBank thành công cho đơn {req.MaHdb}!",
                data = result
            });
        }

        /// <summary>
        /// Xem lịch sử các Webhook ngân hàng đã nhận
        /// </summary>
        [HttpGet("bank/history")]
        public IActionResult GetWebhookHistory()
        {
            var history = _paymentService.GetRecentTransactions();
            return Ok(history);
        }
    }
}
