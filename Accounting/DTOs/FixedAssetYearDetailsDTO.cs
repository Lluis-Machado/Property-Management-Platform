

namespace AccountingAPI.DTOs
{
    public class FixedAssetYearDetailsDTO 
    {
        public Guid Id { get; set; }
        public Guid InvoiceLineId { get; set; }
        public string Description { get; set; }
        public DateTime CapitalizationDate { get; set; }
        public double AcquisitionAndProductionCosts { get; set; }
        public double DepreciationPercentagePerYear { get; set; }
        public int Year { get; set; }
        public double DepreciationNumberOfDays { get; set; }
        public double DepreciationBeginningYear { get; set; }
        public double DepreciationEndOfYear { get; set; }
        public double NetBookValueBeginningYear { get; set; }
        public double NetBookValueEndOfYear { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModificationAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModificationBy { get; set; }
    }
}
