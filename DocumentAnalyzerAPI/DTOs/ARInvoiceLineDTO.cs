

namespace DocumentAnalyzerAPI.DTOs
{
    public class ARInvoiceLineDTO
    {
        public string? Description { get; set; }
        public decimal? Tax { get; set; }
        public int?  Quantity { get; set; }
        public decimal?  UnitPrice { get; set; }
        public DateTime?  ServiceDateFrom { get; set; }
        public DateTime?  ServiceDateTo { get; set; }
    }
}
