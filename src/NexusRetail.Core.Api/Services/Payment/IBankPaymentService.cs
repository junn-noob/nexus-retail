using NexusRetail.Core.Api.DTOs;

namespace NexusRetail.Core.Api.Services.Payment
{
    public interface IBankPaymentService
    {
        PaymentStatusDto ProcessWebhook(BankWebhookDto dto);
        PaymentStatusDto GetPaymentStatus(string maHdb, decimal? expectedAmount = null);
        void RegisterExpectedAmount(string maHdb, decimal amount);
        PaymentStatusDto SimulatePayment(string maHdb, decimal amount);
        IEnumerable<BankWebhookDto> GetRecentTransactions();
    }
}
