

namespace AccountingAPI.DTOs
{
    public class UpdateARInvoiceDTO
    {
        public string RefNumber { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
        public double GrossAmount { get; set; }
        public double NetAmount { get; set; }
        public List<UpdateARInvoiceLineDTO> InvoiceLines { get; set; }   
    }
}
