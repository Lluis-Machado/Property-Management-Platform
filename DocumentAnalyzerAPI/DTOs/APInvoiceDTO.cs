
namespace DocumentAnalyzerAPI.DTOs
{
    public class APInvoiceDTO
    {
        public BusinessPartnerDTO BusinessPartner { get; set; }
        public string? RefNumber { get; set; }
        public DateTime? Date { get; set; }
        public string? Currency { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? TotalBaseAmount { get; set; }

        public decimal? TotalTax { get; set; }
        public decimal? TotalTaxPercentage { get; set; }

        public List<APInvoiceLineDTO> InvoiceLines { get; set; }
 


        public APInvoiceDTO()
        {
            InvoiceLines = new List<APInvoiceLineDTO>();
            BusinessPartner = new();
        }
    }
}
