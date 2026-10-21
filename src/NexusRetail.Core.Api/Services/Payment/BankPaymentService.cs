using NexusRetail.Core.Api.DTOs;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace NexusRetail.Core.Api.Services.Payment
{
    public class BankPaymentService : IBankPaymentService
    {
        private readonly ConcurrentDictionary<string, PaymentStatusDto> _payments = new(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentQueue<BankWebhookDto> _logs = new();
        private readonly ILogger<BankPaymentService> _logger;

        public BankPaymentService(ILogger<BankPaymentService> logger)
        {
            _logger = logger;
        }

        public PaymentStatusDto ProcessWebhook(BankWebhookDto dto)
        {
            _logs.Enqueue(dto);
            _logger.LogInformation("[Webhook] Nhận biến động số dư từ {Gateway}: +{Amount:N0}đ | Nội dung: '{Content}'",
                dto.Gateway, dto.TransferAmount, dto.Content);

            // Bóc tách mã đơn hàng HDB... từ nội dung chuyển khoản
            // Ví dụ: "DUYMOBILE HDB01", "HDB12 CHUYEN TIEN", "DUYMOBILE HDB142530", "DUY MOBILE HDB 05"
            string maHdb = "";
            var matchExact = Regex.Match(dto.Content, @"\b(HDB\w+)\b", RegexOptions.IgnoreCase);
            if (matchExact.Success)
            {
                maHdb = matchExact.Groups[1].Value.ToUpper();
            }
            else
            {
                var match = Regex.Match(dto.Content, @"HDB\s*(\d+)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    maHdb = "HDB" + match.Groups[1].Value;
                }
                else
                {
                    var matchAlt = Regex.Match(dto.Content, @"(HD\w+)", RegexOptions.IgnoreCase);
                    if (matchAlt.Success)
                    {
                        maHdb = matchAlt.Groups[1].Value.ToUpper();
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(maHdb))
            {
                _logger.LogWarning("[Webhook] Không tìm thấy mã đơn hàng HDB trong nội dung: '{Content}'", dto.Content);
                return new PaymentStatusDto
                {
                    IsPaid = false,
                    Message = "Giao dịch không chứa mã đơn hàng HDB hợp lệ"
                };
            }

            var status = new PaymentStatusDto
            {
                MaHdb = maHdb,
                IsPaid = true,
                PaidAmount = dto.TransferAmount,
                TransactionCode = dto.ReferenceCode ?? $"MB_{DateTime.UtcNow.Ticks}",
                PaidAt = DateTime.Now,
                Message = $"Thanh toán thành công qua {dto.Gateway} cho đơn {maHdb}!"
            };

            _payments[maHdb] = status;
            _logger.LogInformation("[Webhook] ✅ ĐÃ XÁC NHẬN THANH TOÁN cho đơn {MaHdb} số tiền {Amount:N0}đ", maHdb, dto.TransferAmount);

            return status;
        }

        public PaymentStatusDto GetPaymentStatus(string maHdb)
        {
            if (string.IsNullOrWhiteSpace(maHdb))
            {
                return new PaymentStatusDto { IsPaid = false, Message = "Mã đơn hàng không hợp lệ" };
            }

            if (_payments.TryGetValue(maHdb.Trim(), out var status))
            {
                return status;
            }

            return new PaymentStatusDto
            {
                MaHdb = maHdb,
                IsPaid = false,
                Message = "Đang chờ thanh toán..."
            };
        }

        public PaymentStatusDto SimulatePayment(string maHdb, decimal amount)
        {
            maHdb = maHdb.Trim().ToUpper();
            var fakeWebhook = new BankWebhookDto
            {
                Gateway = "MBBank (Simulate)",
                AccountNumber = "09999999900",
                TransferAmount = amount,
                Content = $"DUYMOBILE {maHdb} CHUYEN TIEN",
                ReferenceCode = $"SIM_{DateTime.UtcNow.Ticks}",
                TransactionDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
            };

            return ProcessWebhook(fakeWebhook);
        }

        public IEnumerable<BankWebhookDto> GetRecentTransactions()
        {
            return _logs.Reverse().Take(50);
        }
    }
}
