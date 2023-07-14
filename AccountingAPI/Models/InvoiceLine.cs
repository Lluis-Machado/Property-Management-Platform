
namespace AccountingAPI.Models
{
    public class InvoiceLine : BaseModel
    {
        public Guid InvoiceId { get; set; }
        public string Description { get; set; }
        public decimal Tax { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime? ServiceDateFrom { get; set; }
        public DateTime? ServiceDateTo { get; set; }

        public InvoiceLine()
        {
            Description = string.Empty;
        }
    }
}
