namespace AccountingAPI.DTOs
{
    public class APInvoiceDTO
    {
        public Guid Id { get; set; }
        public BusinessPartnerDTO BusinessPartner { get; set; }
        public string RefNumber { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal NetAmount { get; set; }
        public List<APInvoiceLineDTO> InvoiceLines { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModificationAt { get; set; }
        public string CreatedBy { get; set; }
        public string LastModificationBy { get; set; }

        public APInvoiceDTO()
        {
            InvoiceLines = new List<APInvoiceLineDTO>();
            BusinessPartner = new();
            RefNumber = string.Empty;
            Currency = string.Empty;
            CreatedBy = string.Empty;
            LastModificationBy = string.Empty;
        }
    }
}
