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

            // Tra cứu đơn hàng thực tế từ DB để đồng bộ số tiền kỳ vọng và số tiền đã nhận trước đó
            decimal exp = 0m;
            decimal persistedPaid = 0m;
            if (_expectedAmounts.TryGetValue(maHdb, out var registeredExp) && registeredExp > 0)
            {
                exp = registeredExp;
            }

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<RetailDbContext>();
                var hdb = db.HoaDonBans.FirstOrDefault(h => h.MaHdb == maHdb);
                if (hdb != null)
                {
                    if (exp <= 0 && hdb.TongTien > 0)
                    {
                        exp = hdb.TongTien;
                        _expectedAmounts[maHdb] = exp;
                    }
                    persistedPaid = hdb.SoTienDaThanhToan;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("[Webhook] Không thể tra cứu DB cho đơn {MaHdb}: {Error}", maHdb, ex.Message);
            }

            // Tính lũy kế nhằm hỗ trợ khách hàng chuyển khoản nhiều lần
            decimal previousPaid = 0m;
            if (_payments.TryGetValue(maHdb, out var existingStatus))
            {
                previousPaid = existingStatus.PaidAmount;
            }
            previousPaid = Math.Max(previousPaid, persistedPaid);
            decimal totalPaid = previousPaid + dto.TransferAmount;

            bool isPaid = false;
            string message;

            if (exp > 0)
            {
                if (totalPaid < exp)
                {
                    isPaid = false;
                    message = $"Khách chuyển thiếu tiền: Đã nhận {totalPaid:N0}đ / Cần thanh toán {exp:N0}đ (Còn thiếu {(exp - totalPaid):N0}đ)";
                    _logger.LogWarning("[Webhook] ⚠️ ĐƠN {MaHdb} CHUYỂN THIẾU TIỀN: Đã nhận {Paid:N0}đ / Cần {Exp:N0}đ (Thiếu {Missing:N0}đ)",
                        maHdb, totalPaid, exp, exp - totalPaid);
                }
                else
                {
                    isPaid = true;
                    message = $"Thanh toán thành công qua {dto.Gateway} cho đơn {maHdb}! (Đã nhận {totalPaid:N0}đ / Cần {exp:N0}đ)";
                    _logger.LogInformation("[Webhook] ✅ ĐÃ XÁC NHẬN THANH TOÁN ĐỦ cho đơn {MaHdb} ({Paid:N0}đ / {Exp:N0}đ)",
                        maHdb, totalPaid, exp);
                }
            }
            else
            {
                // Không tự động xác nhận IsPaid=true cho đơn chưa đăng ký hoặc chưa tìm thấy trong DB
                isPaid = false;
                message = $"Đã nhận {totalPaid:N0}đ qua {dto.Gateway} cho đơn {maHdb} (đang chờ đối soát số tiền hợp lệ).";
                _logger.LogInformation("[Webhook] Đã nhận {Paid:N0}đ cho đơn {MaHdb} (chưa có ExpectedAmount hợp lệ để xác nhận đủ)", totalPaid, maHdb);
            }

            string txCode = string.IsNullOrWhiteSpace(dto.ReferenceCode)
                ? $"MB_{DateTime.UtcNow.Ticks}"
                : dto.ReferenceCode.Trim();

            // Lưu vết giao dịch xuống DB để không mất trạng thái khi khởi động lại dịch vụ
            try
            {
                using var persistScope = _scopeFactory.CreateScope();
                var db = persistScope.ServiceProvider.GetRequiredService<RetailDbContext>();
                var hdb = db.HoaDonBans.FirstOrDefault(h => h.MaHdb == maHdb);
                if (hdb != null)
                {
                    hdb.SoTienDaThanhToan = totalPaid;
                    hdb.MaGiaoDich = txCode;
                    if (isPaid)
                    {
                        hdb.TrangThaiThanhToan = "DaThanhToan";
                        if (string.Equals(hdb.TrangThaiDonHang, "ChoXacNhan", StringComparison.OrdinalIgnoreCase))
                        {
                            hdb.TrangThaiDonHang = "DangGiao";
                        }
                    }
                    else if (totalPaid > 0 && !string.Equals(hdb.TrangThaiThanhToan, "DaThanhToan", StringComparison.OrdinalIgnoreCase))
                    {
                        hdb.TrangThaiThanhToan = "ThanhToanMotPhan";
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("[Webhook] Không thể cập nhật DB cho đơn {MaHdb}: {Error}", maHdb, ex.Message);
            }

            var status = new PaymentStatusDto
            {
                MaHdb = maHdb,
                IsPaid = isPaid,
                PaidAmount = totalPaid,
                ExpectedAmount = exp,
                TransactionCode = txCode,
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

            maHdb = maHdb.Trim().ToUpperInvariant();

            if (expectedAmount.HasValue && expectedAmount.Value > 0)
            {
                _expectedAmounts[maHdb] = expectedAmount.Value;
            }

            if (_payments.TryGetValue(maHdb, out var status))
            {
                decimal evalExpected = expectedAmount.HasValue && expectedAmount.Value > 0
                    ? expectedAmount.Value
                    : (status.ExpectedAmount > 0 ? status.ExpectedAmount
                        : (_expectedAmounts.TryGetValue(maHdb, out var cachedExp) ? cachedExp : 0m));

                if (evalExpected > 0)
                {
                    status.ExpectedAmount = evalExpected;
                    if (status.PaidAmount < evalExpected)
                    {
                        status.IsPaid = false;
                        status.Message = $"Khách chuyển thiếu tiền: Đã nhận {status.PaidAmount:N0}đ / Cần {evalExpected:N0}đ (Còn thiếu {(evalExpected - status.PaidAmount):N0}đ)";
                    }
                    else
                    {
                        status.IsPaid = true;
                        status.Message = $"Thanh toán đủ {status.PaidAmount:N0}đ cho đơn {maHdb}!";
                    }
                }
                else
                {
                    status.IsPaid = false;
                }
                return status;
            }

            // Fallback đọc từ DB nếu service vừa khởi động lại hoặc đơn đã thanh toán trước đó
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<RetailDbContext>();
                var hdb = db.HoaDonBans.AsNoTracking().FirstOrDefault(h => h.MaHdb == maHdb);
                if (hdb != null)
                {
                    decimal expDb = expectedAmount.HasValue && expectedAmount.Value > 0
                        ? expectedAmount.Value
                        : hdb.TongTien;
                    decimal paidDb = hdb.SoTienDaThanhToan;
                    bool dbPaid = string.Equals(hdb.TrangThaiThanhToan, "DaThanhToan", StringComparison.OrdinalIgnoreCase)
                        || (expDb > 0 && paidDb >= expDb);

                    string dbMsg = dbPaid
                        ? $"Đơn {maHdb} đã thanh toán {Math.Max(paidDb, expDb):N0}đ!"
                        : (paidDb > 0 && expDb > 0
                            ? $"Khách chuyển thiếu tiền: Đã nhận {paidDb:N0}đ / Cần {expDb:N0}đ (Còn thiếu {(expDb - paidDb):N0}đ)"
                            : "Đang chờ thanh toán...");

                    return new PaymentStatusDto
                    {
                        MaHdb = maHdb,
                        IsPaid = dbPaid,
                        PaidAmount = paidDb > 0 ? paidDb : (dbPaid ? expDb : 0m),
                        ExpectedAmount = expDb,
                        TransactionCode = hdb.MaGiaoDich,
                        Message = dbMsg
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("[Webhook] Không thể đọc trạng thái DB cho đơn {MaHdb}: {Error}", maHdb, ex.Message);
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
