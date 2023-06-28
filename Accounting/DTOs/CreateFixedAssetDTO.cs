
namespace AccountingAPI.DTOs
{
    public class CreateFixedAssetDTO
    {
        public Guid InvoiceLineId { get; set; }
        public string Description { get; set; }
        public DateTime CapitalizationDate { get; set; }
        public decimal AcquisitionAndProductionCosts { get; set; }
        public decimal DepreciationPercentagePerYear { get; set; }

        public CreateFixedAssetDTO()
        {
            Description = string.Empty;
        }
    }
}
