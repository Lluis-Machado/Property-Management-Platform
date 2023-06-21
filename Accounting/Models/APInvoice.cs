namespace AccountingAPI.Models
{
    public class APInvoice : Invoice
    {
        public Guid VendorId { get; set; }
    }
}
