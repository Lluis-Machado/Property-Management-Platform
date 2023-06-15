namespace AccountingAPI.DTOs
{
    public class ARInvoiceLineDTO
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public string? Description { get; set; }
        public double Tax { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double TotalPrice { get; set; }
        public DateTime ServiceDateFrom { get; set; }
        public DateTime ServiceDateTo { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModificationAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModificationBy { get; set; }

        public ARInvoiceLineDTO()
        {
        }
    }
}
