using System.Text.Json.Serialization;

namespace AccountingAPI.Models
{
    public class ARInvoice :Invoice
    {
        public Guid VendorId { get; set; }

        [JsonConstructor]
        public ARInvoice()
        {
        }
    }
}
