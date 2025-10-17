namespace bedayaacc.Models
{
    public class ExamOrder
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int ExamId { get; set; }
        public string Currency { get; set; } = "EGP";
        public decimal PriceAtPurchase { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public string Status { get; set; } = "PENDING";
        public string? Provider { get; set; }
        public string? ProviderRef { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public bool IsDeleted { get; set; }

        // جديد للتحويل البنكي
        public string PaymentMethod { get; set; } = "BANK_TRANSFER";
        public string? ReceiptFileName { get; set; }
        public string? ReceiptUrl { get; set; }
        public DateTime? ReceiptUploadedAt { get; set; }
        public string? BankRef { get; set; }
    }

}
