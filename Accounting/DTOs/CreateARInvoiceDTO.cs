
namespace AccountingAPI.DTOs
{
    public class CreateARInvoiceDTO
    {
        public string RefNumber { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
        public List<CreateARInvoiceLineDTO> InvoiceLines { get; set; }   
    }
}
