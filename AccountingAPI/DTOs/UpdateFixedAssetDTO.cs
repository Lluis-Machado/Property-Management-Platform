
namespace AccountingAPI.DTOs
{
    public class UpdateFixedAssetDTO
    {
        public string Description { get; set; }
        public DateTime CapitalizationDate { get; set; }
        public decimal AcquisitionAndProductionCosts { get; set; }
        public decimal DepreciationPercentagePerYear { get; set; }

        public UpdateFixedAssetDTO()
        {
            Description = string.Empty;
        }
    }
}
