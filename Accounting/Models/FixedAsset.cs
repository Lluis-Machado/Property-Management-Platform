using System.Text.Json.Serialization;

namespace Accounting.Models
{
    public class FixedAsset
    {
        public Guid InvoiceId { get; set; }
        public string Name { get; set; }
        public DateTime ActivationDate { get; set; }
        public double ActivationAmount { get; set; }
        public Guid DepreciationConfigId { get; set; }
        public double DepreciationAmount { get; set; } // Taken from the DepreciationConfig, or custom set

        public Guid Id { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
        public string LastModificationByUser { get; set; }

        [JsonConstructor]
        public FixedAsset() 
        {
            Name = string.Empty;
            LastModificationByUser = string.Empty;
        }
    }
}
