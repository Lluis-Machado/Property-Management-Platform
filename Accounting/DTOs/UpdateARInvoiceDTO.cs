

namespace AccountingAPI.DTOs
{
    public class UpdateARInvoiceDTO
    {
        public string? RefNumber { get; set; }
        public DateTime Date { get; set; }
        public string? Currency { get; set; }
        public List<UpdateARInvoiceLineDTO> InvoiceLines { get; set; }

        public UpdateARInvoiceDTO()
        {
            InvoiceLines = new List<UpdateARInvoiceLineDTO>();
        }
    }
}
