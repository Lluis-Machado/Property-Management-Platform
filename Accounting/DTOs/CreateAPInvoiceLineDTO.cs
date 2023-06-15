using System.Text.Json.Serialization;

namespace AccountingAPI.DTOs
{
    public class CreateAPInvoiceLineDTO
    {
        public string Description { get; set; }
        public double Tax { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public string ExpenseCategoryType { get; set; }
        public string ExpenseCategoryDescription { get; set; }
        public double DepreciationRatePerYear { get; set; }
        public DateTime ServiceDateFrom { get; set; }
        public DateTime ServiceDateTo { get; set; }

        [JsonConstructor]
        public CreateAPInvoiceLineDTO()
        {
            Description = String.Empty;
            ExpenseCategoryType = String.Empty;
            ExpenseCategoryDescription = String.Empty;
        }
    }
}
