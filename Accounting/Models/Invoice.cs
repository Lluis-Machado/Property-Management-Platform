

namespace AccountingAPI.Models
{
    public class Invoice : BaseModel
    {
        public Guid BusinessPartnerId { get; set; }
        public string RefNumber { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal NetAmount { get; set; }

        public Invoice()
        {
            RefNumber = string.Empty;
            Currency = String.Empty;
        }
    }
}
