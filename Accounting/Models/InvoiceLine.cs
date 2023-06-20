
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
        public DateTime? ServiceDateFrom { get; set; }
        public DateTime? ServiceDateTo { get; set; }
    }
}
