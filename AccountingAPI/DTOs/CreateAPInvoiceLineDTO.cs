

namespace AccountingAPI.DTOs
{
    public class CreateAPInvoiceLineDTO
    {
        public string? Description { get; set; }
        public decimal Tax { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public Guid ExpenseCategoryId { get; set; }
        public decimal? DepreciationRatePerYear { get; set; }
        public DateTime? ServiceDateFrom { get; set; }
        public DateTime? ServiceDateTo { get; set; }

        public CreateAPInvoiceLineDTO()
        {
            Description = string.Empty;
        }
    }
}
