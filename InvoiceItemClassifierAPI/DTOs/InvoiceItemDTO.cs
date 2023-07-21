using Microsoft.ML.Data;

namespace InvoiceItemClassifierAPI.DTOs
{
    public class InvoiceItemDTO
    {
        public string VendorName { get; set; }

        public string VendorTaxId { get; set; }

        public string InvoiceLineDescription { get; set; }

        public bool HasPeriod { get; set; }
    }
}
