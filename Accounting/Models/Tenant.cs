using System.Text.Json.Serialization;

namespace Accounting.Models
{
    public class Tenant :IAuditable
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModificationAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModificationBy { get; set; }

        [JsonConstructor]
        public Tenant()
        {
            Name = string.Empty;
            LastModificationBy = string.Empty;
        }
    }
}
