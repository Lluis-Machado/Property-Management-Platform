
namespace AccountingAPI.DTOs
{
    public class CreateAPInvoiceDTO
    {
        public string RefNumber { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
        public List<CreateAPInvoiceLineDTO> InvoiceLines { get; set; }
    }
}
