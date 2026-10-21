namespace NexusRetail.Core.Api.DTOs
{
    public class BankWebhookDto
    {
        public string Gateway { get; set; } = "MBBank";
        public string? AccountNumber { get; set; }
        public decimal TransferAmount { get; set; }
        public string Content { get; set; } = "";
        public string? ReferenceCode { get; set; }
        public string? TransactionDate { get; set; }
    }

    public class PaymentStatusDto
    {
        public string MaHdb { get; set; } = "";
        public bool IsPaid { get; set; }
        public decimal PaidAmount { get; set; }
        public string? TransactionCode { get; set; }
        public DateTime? PaidAt { get; set; }
        public string Message { get; set; } = "";
    }

    public class SimulatePaymentRequest
    {
        public string MaHdb { get; set; } = "";
        public decimal Amount { get; set; }
    }
}
