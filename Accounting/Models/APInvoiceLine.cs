
namespace AccountingAPI.Models
{
    public class APInvoiceLine :InvoiceLine
    {
        public Guid ExpenseCategoryId { get; set; }
        public Guid? FixedAssetId { get; set; }
    }
}
