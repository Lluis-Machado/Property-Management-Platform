

namespace DocumentAnalyzerAPI.DTOs
{
    public class APInvoiceLineDTO
    {
        public string? Description { get; set; }
        public decimal? Tax { get; set; }
        public Double? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public Guid? ExpenseCategoryId { get; set; }
        public decimal? DepreciationRatePerYear { get; set; }
        public DateTime? ServiceDateFrom { get; set; }
        public DateTime? ServiceDateTo { get; set; }
    }
}
