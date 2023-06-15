using AccountingAPI.Models;

namespace AccountingAPI.Models
{
    public class InvoiceLine :BaseModel
    {
        public Guid InvoiceId { get; set; }
        public string Description { get; set; }
        public double Tax { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }
        public Guid ExpenseCategoryId { get; set; }
        public DateTime? ServiceDateFrom { get; set; }
        public DateTime ServiceDateTo { get; set; }
        public Guid? FixedAssetId { get; set; }

        public InvoiceLine()
        {
            Description = string.Empty;
            LastModificationBy = string.Empty;
        }
    }
}
