using System.Text.Json.Serialization;

namespace Accounting.Models
{
    public class Loan
    {
        public Guid BusinessPartnerID { get; set; }
        public string Concept { get; set; }
        public double Amount { get; set; }
        public double AmountPaid { get; set; }

        public Guid Id { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
        public string LastModificationByUser { get; set; }

        [JsonConstructor]
        public Loan()
        {
            Concept = string.Empty;
            Amount = 0;
            AmountPaid = 0;
            LastModificationByUser = string.Empty;
        }
    }
}
