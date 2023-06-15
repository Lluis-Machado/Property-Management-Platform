using AccountingAPI.Models;
using System.Text.Json.Serialization;

namespace AccountingAPI.Models
{
    public class Invoice :BaseModel
    {
        public Guid BusinessPartnerId { get; set; }
        public string RefNumber { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
        public double GrossAmount { get; set; }
        public double NetAmount { get; set; }

        [JsonConstructor]
        public Invoice()
        {
            RefNumber = string.Empty;
            Currency = string.Empty;
            GrossAmount = 0;
            NetAmount = 0;
        }
    }
}
