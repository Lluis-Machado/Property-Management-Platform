namespace AccountingAPI.DTOs
{
    public class UpdateInvoiceLineDTO
    {
        public Guid? Id { get; set; }
        public string? Description { get; set; }
        public double Tax { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public string? ExpenseCategoryType { get; set; }
        public string? ExpenseCategoryDescription { get; set; }
        public double DepreciationRatePerYear { get; set; }
        public DateTime ServiceDateFrom { get; set; }
        public DateTime ServiceDateTo { get; set; }

        public UpdateInvoiceLineDTO()
        {
        }
    }
}
