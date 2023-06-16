
namespace AccountingAPI.DTOs
{
    public class CreateARInvoiceDTO
    {
        public string RefNumber { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
        public double GrossAmount { get; set; }
        public double NetAmount { get; set; }
        public List<CreateARInvoiceLineDTO> InvoiceLines { get; set; }   
    }
}
