using Dapper.Contrib.Extensions;
using System.Text.Json.Serialization;

namespace Accounting.Models
{
    public class DepreciationConfig
    {
        public string Name { get; set; }
        public string Type { get; set; }    // TODO: Linear, progressive, etc... ? Save as enum?
        public double DepreciationPercent { get; set; }
        public int MaxYears { get; set; }

        // These two bools indicate whether the % and years can be user-defined - False by default
        public bool CustomSetPercent { get; set; } = false;
        public bool CustomSetYears { get; set; } = false;

        [ExplicitKey]
        public Guid Id { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
        public string LastModificationByUser { get; set; }

        [JsonConstructor]
        public DepreciationConfig()
        {
            Name = string.Empty;
            Type = string.Empty;
            LastModificationByUser = string.Empty;
        }
    }
}
