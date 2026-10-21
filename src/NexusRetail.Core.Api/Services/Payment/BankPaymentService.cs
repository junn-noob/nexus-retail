using Microsoft.EntityFrameworkCore;
using NexusRetail.Core.Api.Data;
using NexusRetail.Core.Api.DTOs;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace NexusRetail.Core.Api.Services.Payment
{
    public class BankPaymentService : IBankPaymentService
    {
        private readonly ConcurrentDictionary<string, PaymentStatusDto> _payments = new(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, decimal> _expectedAmounts = new(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentQueue<BankWebhookDto> _logs = new();
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BankPaymentService> _logger;

        public BankPaymentService(IServiceScopeFactory scopeFactory, ILogger<BankPaymentService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public void RegisterExpectedAmount(string maHdb, decimal amount)
        {
            if (!string.IsNullOrWhiteSpace(maHdb) && amount > 0)
            {
                _expectedAmounts[maHdb.Trim()] = amount;
            }
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

            // 1. Xác định số tiền cần thanh toán của đơn hàng (ExpectedAmount)
            decimal exp = 0;
            if (_expectedAmounts.TryGetValue(maHdb, out var registeredExp) && registeredExp > 0)
            {
                exp = registeredExp;
            }
            else
            {
                // Tra cứu trực tiếp từ Database HoaDonBan nếu đơn hàng đã được lưu DB
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<RetailDbContext>();
                    var hdb = db.HoaDonBans.AsNoTracking().FirstOrDefault(h => h.MaHdb == maHdb);
                    if (hdb != null && hdb.TongTien > 0)
                    {
                        exp = hdb.TongTien;
                        _expectedAmounts[maHdb] = exp;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("[Webhook] Không thể tra cứu DB cho đơn {MaHdb}: {Error}", maHdb, ex.Message);
                }
            }

            // 2. Tính lũy kế số tiền đã thanh toán (hỗ trợ trường hợp khách chuyển làm nhiều lần)
            decimal previousPaid = 0m;
            if (_payments.TryGetValue(maHdb, out var existingStatus))
            {
                previousPaid = existingStatus.PaidAmount;
            }
            decimal totalPaid = previousPaid + dto.TransferAmount;

            // 3. SO SÁNH SỐ TIỀN CHUYỂN KHOẢN VỚI SỐ TIỀN CẦN THANH TOÁN
            bool isPaid = false;
            string message;

            if (exp > 0)
            {
                if (totalPaid < exp)
                {
                    // Chuyển thiếu tiền -> Chưa xác nhận hoàn tất thanh toán
                    isPaid = false;
                    message = $"Khách chuyển thiếu tiền: Đã nhận {totalPaid:N0}đ / Cần thanh toán {exp:N0}đ (Còn thiếu {(exp - totalPaid):N0}đ)";
                    _logger.LogWarning("[Webhook] ⚠️ ĐƠN {MaHdb} CHUYỂN THIẾU TIỀN: Đã nhận {Paid:N0}đ / Cần {Exp:N0}đ (Thiếu {Missing:N0}đ)",
                        maHdb, totalPaid, exp, exp - totalPaid);
                }
                else
                {
                    // Chuyển đủ hoặc thừa tiền -> Xác nhận thanh toán thành công
                    isPaid = true;
                    message = $"Thanh toán thành công qua {dto.Gateway} cho đơn {maHdb}! (Đã nhận {totalPaid:N0}đ / Cần {exp:N0}đ)";
                    _logger.LogInformation("[Webhook] ✅ ĐÃ XÁC NHẬN THANH TOÁN ĐỦ cho đơn {MaHdb} ({Paid:N0}đ / {Exp:N0}đ)",
                        maHdb, totalPaid, exp);
                }
            }
            else
            {
                // Chưa có số tiền dự kiến trong hệ thống: Ghi nhận số tiền nhận được, Client POS sẽ so khớp tiếp
                isPaid = true;
                message = $"Đã nhận {totalPaid:N0}đ qua {dto.Gateway} cho đơn {maHdb}!";
                _logger.LogInformation("[Webhook] Đã nhận {Paid:N0}đ cho đơn {MaHdb} (Đang chờ client xác thực số tiền)", totalPaid, maHdb);
            }

            var status = new PaymentStatusDto
            {
                MaHdb = maHdb,
                IsPaid = isPaid,
                PaidAmount = totalPaid,
                ExpectedAmount = exp,
                TransactionCode = dto.ReferenceCode ?? $"MB_{DateTime.UtcNow.Ticks}",
                PaidAt = DateTime.Now,
                Message = message
            };

            _payments[maHdb] = status;
            return status;
        }

        public PaymentStatusDto GetPaymentStatus(string maHdb, decimal? expectedAmount = null)
        {
            if (string.IsNullOrWhiteSpace(maHdb))
            {
                return new PaymentStatusDto { IsPaid = false, Message = "Mã đơn hàng không hợp lệ" };
            }

            maHdb = maHdb.Trim();

            // Nếu caller truyền expectedAmount vào, cập nhật kỳ vọng
            if (expectedAmount.HasValue && expectedAmount.Value > 0)
            {
                _expectedAmounts[maHdb] = expectedAmount.Value;
            }

            if (_payments.TryGetValue(maHdb, out var status))
            {
                // Nếu caller truyền expectedAmount và trước đó chưa kiểm tra hoặc cần đánh giá lại
                if (expectedAmount.HasValue && expectedAmount.Value > 0)
                {
                    status.ExpectedAmount = expectedAmount.Value;
                    if (status.PaidAmount < expectedAmount.Value)
                    {
                        status.IsPaid = false;
                        status.Message = $"Khách chuyển thiếu tiền: Đã nhận {status.PaidAmount:N0}đ / Cần {expectedAmount.Value:N0}đ (Còn thiếu {(expectedAmount.Value - status.PaidAmount):N0}đ)";
                    }
                    else
                    {
                        status.IsPaid = true;
                        status.Message = $"Thanh toán đủ {status.PaidAmount:N0}đ cho đơn {maHdb}!";
                    }
                }
                return status;
            }

            return new PaymentStatusDto
            {
                MaHdb = maHdb,
                IsPaid = false,
                ExpectedAmount = expectedAmount ?? 0,
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
