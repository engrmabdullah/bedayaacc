// Models/BankAccount.cs
namespace bedayaacc.Models
{
    public class BankAccount
    {
        public int BankAccountId { get; set; }
        public string BankName { get; set; } = "";
        public string AccountName { get; set; } = "";
        public string AccountNumber { get; set; } = "";
        public string? IBAN { get; set; }
        public string? SwiftCode { get; set; }
        public string? Branch { get; set; }
        public string Currency { get; set; } = "EGP";
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public int DisplayOrder { get; set; }
        public string? Notes { get; set; }
    }
}
