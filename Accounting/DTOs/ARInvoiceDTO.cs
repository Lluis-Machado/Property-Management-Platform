using System.Text.Json.Serialization;

namespace AccountingAPI.DTOs
{
    public class ARInvoiceDTO
    {
        public Guid Id { get; set; }
        public string RefNumber { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
        public double GrossAmount { get; set; }
        public double NetAmount { get; set; }
        public List<ARInvoiceLineDTO> InvoiceLines { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModificationAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModificationBy { get; set; }

        [JsonConstructor]
        public ARInvoiceDTO()
        {
            RefNumber = string.Empty;
            Currency = string.Empty;
            GrossAmount = 0;
            NetAmount = 0;
            InvoiceLines = new List<ARInvoiceLineDTO>();
        }
    }
}
