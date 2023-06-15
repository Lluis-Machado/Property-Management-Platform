using System.Text.Json.Serialization;

namespace AccountingAPI.DTOs
{
    public class UpdateAPInvoiceDTO
    {
        public string RefNumber { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
        public double GrossAmount { get; set; }
        public double NetAmount { get; set; }
        public List<UpdateAPInvoiceLineDTO> InvoiceLines { get; set; }   

        [JsonConstructor]
        public UpdateAPInvoiceDTO()
        {
            RefNumber = string.Empty;
            Currency = string.Empty;
            GrossAmount = 0;
            NetAmount = 0;
            InvoiceLines = new List<UpdateAPInvoiceLineDTO>();
        }
    }
}
