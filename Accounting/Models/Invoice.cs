using System.Data.SqlTypes;
using System.Text.Json.Serialization;

namespace Accounting.Models
{
    public class Invoice
    {
        public Guid BusinessPartnerId { get; set; }
        public string RefNumber { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
        public double GrossAmount { get; set; }
        public double NetAmount { get; set; }

        public Guid Id { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
        public string LastModificationByUser { get; set; }

        [JsonConstructor]
        public Invoice()
        {
            RefNumber = string.Empty;
            Currency = string.Empty;
            LastModificationByUser = string.Empty;
        }
    }
}
