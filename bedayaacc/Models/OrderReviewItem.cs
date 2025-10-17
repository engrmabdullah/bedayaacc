namespace bedayaacc.Models
{
    // DTO للعرض
    public class OrderReviewItem
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public int ExamId { get; set; }
        public string ExamTitle { get; set; } = "";
        public decimal PaidAmount { get; set; }
        public string Currency { get; set; } = "EGP";
        public string Status { get; set; } = "PAID_UNVERIFIED";
        public string? ReceiptUrl { get; set; }
        public string? BankRef { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
