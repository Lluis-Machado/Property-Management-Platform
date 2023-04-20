using System.Text.Json.Serialization;

namespace Accounting.Models
{
    public class DepreciationConfig
    {
        public string Type { get; set; }
        public double DepreciationPercent { get; set; }


        public Guid Id { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
        public string LastModificationByUser { get; set; }

        [JsonConstructor]
        public DepreciationConfig()
        {
            Type = string.Empty;
            LastModificationByUser = string.Empty;
        }
    }
}
