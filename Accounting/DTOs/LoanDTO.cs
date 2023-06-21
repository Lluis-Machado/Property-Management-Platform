

namespace AccountingAPI.DTOs
{
    public class LoanDTO
    {
        public Guid Id { get; set; }
        public Guid BusinessPartnerId { get; set; }
        public string? Concept { get; set; }
        public double Amount { get; set; }
        public double AmountPaid { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModificationAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModificationBy { get; set; }
    }
}
