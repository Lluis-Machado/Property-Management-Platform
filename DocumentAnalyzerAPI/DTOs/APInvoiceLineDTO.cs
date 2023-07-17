

namespace DocumentAnalyzerAPI.DTOs
{
    public class APInvoiceLineDTO
    {
        public FieldInfo<string> Description { get; set; }
        public FieldInfo<decimal> Tax { get; set; }
        public FieldInfo<int>  Quantity { get; set; }
        public FieldInfo<decimal>  UnitPrice { get; set; }
        public FieldInfo<Guid>  ExpenseCategoryId { get; set; }
        public FieldInfo<decimal>  DepreciationRatePerYear { get; set; }
        public FieldInfo<DateTime?>  ServiceDateFrom { get; set; }
        public FieldInfo<DateTime?>  ServiceDateTo { get; set; }

        public APInvoiceLineDTO()
        {
        }
    }
}
