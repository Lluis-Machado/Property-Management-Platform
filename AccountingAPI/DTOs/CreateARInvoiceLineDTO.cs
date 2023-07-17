
namespace AccountingAPI.DTOs
{
    public class CreateARInvoiceLineDTO
    {
        public string Description { get; set; }
        public decimal Tax { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime ServiceDateFrom { get; set; }
        public DateTime ServiceDateTo { get; set; }

        public CreateARInvoiceLineDTO()
        {
            Description = string.Empty;
        }
    }
}
