using System.Text.Json.Serialization;

namespace Accounting.Models
{
    public class FixedAsset :IAuditable
    {
        public Guid Id { get; set; }
        public Guid InvoiceId { get; set; }
        public string Description { get; set; }
        public DateTime CapitalizationDate { get; set; }
        public double AcquisitionAndProductionCosts { get; set; }
        public double DepreciationAmountPercent { get; set; } 
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModificationAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModificationBy { get; set; }

        [JsonConstructor]
        public FixedAsset()
        {
            Description = string.Empty;
            LastModificationBy = string.Empty;
        }
    }
}
