using Microsoft.ML.Data;

namespace InvoiceItemAnalyzerAPI.DTOs
{
    public class ItemDTO
    {
        public string VendorName { get; set; }

        public string VendorTaxId { get; set; }

        public string InvoiceLineDescription { get; set; }

        public bool HasPeriod { get; set; }
    }
}
