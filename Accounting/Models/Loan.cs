using System.Text.Json.Serialization;

namespace Accounting.Models
{
    public class Loan :IAuditable
    {
        public Guid Id { get; set; }
        public Guid BusinessPartnerId { get; set; }
        public string Concept { get; set; }
        public double Amount { get; set; }
        public double AmountPaid { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModificationAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModificationBy { get; set; }

        [JsonConstructor]
        public Loan()
        {
            Concept = string.Empty;
            Amount = 0;
            AmountPaid = 0;
            LastModificationBy = string.Empty;
        }
    }
}
