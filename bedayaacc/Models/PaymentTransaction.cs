namespace bedayaacc.Models
{
    public class PaymentTransaction
    {
        public long TransactionId { get; set; }
        public int OrderId { get; set; }
        public string Provider { get; set; } = "";
        public string? ProviderTxnId { get; set; }
        public string Status { get; set; } = "CREATED";
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";
        public string? RawPayload { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
