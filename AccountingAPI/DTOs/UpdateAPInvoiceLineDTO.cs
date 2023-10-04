namespace AccountingAPI.DTOs
{
    public class UpdateAPInvoiceLineDTO
    {
        public Guid? Id { get; set; }
        public string Description { get; set; }
        public decimal Tax { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ExpenseCategoryType { get; set; }
        public string ExpenseCategoryDescription { get; set; }
        public decimal DepreciationRatePerYear { get; set; }
        public DateTime ServiceDateFrom { get; set; }
        public DateTime ServiceDateTo { get; set; }

        public UpdateAPInvoiceLineDTO()
        {
            Description = string.Empty;
            ExpenseCategoryType = string.Empty;
            ExpenseCategoryDescription = string.Empty;
        }
    }
}
