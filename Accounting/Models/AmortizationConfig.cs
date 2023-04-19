using System.Text.Json.Serialization;

namespace Accounting.Models
{
    public class AmortizationConfig
    {
        public string Type { get; set; }
        public double AmortizationPercent { get; set; }


        public Guid Id { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
        public string LastModificationByUser { get; set; }

        [JsonConstructor]
        public AmortizationConfig()
        {
            Type = string.Empty;
            LastModificationByUser = string.Empty;
        }
    }
}
