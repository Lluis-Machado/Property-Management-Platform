
namespace DocumentAnalyzerAPI.DTOs
{
    public class APInvoiceDTO
    {
        public BusinessPartnerDTO BusinessPartner { get; set; }
        public string? RefNumber { get; set; }
        public DateTime? Date { get; set; }
        public string? Currency { get; set; }
        public List<APInvoiceLineDTO> InvoiceLines { get; set; }

        public APInvoiceDTO()
        {
            InvoiceLines = new List<APInvoiceLineDTO>();
            BusinessPartner = new();
        }
    }
}
