

namespace AccountingAPI.DTOs
{
    public class FixedAssetYearDetailsDTO
    {
        public Guid Id { get; set; }
        public Guid InvoiceLineId { get; set; }
        public string Description { get; set; }
        public DateTime CapitalizationDate { get; set; }
        public decimal AcquisitionAndProductionCosts { get; set; }
        public decimal DepreciationPercentagePerYear { get; set; }
        public int Year { get; set; }
        public decimal DepreciationBeginningYear { get; set; }
        public decimal DepreciationAtYear { get; set; }
        public decimal DepreciationEndOfYear { get; set; }
        public decimal NetBookValueBeginningYear { get; set; }
        public decimal NetBookValueEndOfYear { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModificationAt { get; set; }
        public string CreatedBy { get; set; }
        public string LastModificationBy { get; set; }

        public FixedAssetYearDetailsDTO()
        {
            Description = string.Empty;
            CreatedBy = string.Empty;
            LastModificationBy = string.Empty;
        }

    }
}
