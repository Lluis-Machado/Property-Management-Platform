
namespace DocumentAnalyzerAPI.DTOs
{
    public class ARInvoiceDTO
    {
        public BusinessPartnerDTO BusinessPartner { get; set; }
        public string? RefNumber { get; set; }
        public DateTime? Date { get; set; }
        public string? Currency { get; set; }
        public List<ARInvoiceLineDTO> InvoiceLines { get; set; }
        public decimal? TotalAmount { get; set; }

        public ARInvoiceDTO()
        {
            InvoiceLines = new List<ARInvoiceLineDTO>();
            BusinessPartner = new();
        }
    }
}
