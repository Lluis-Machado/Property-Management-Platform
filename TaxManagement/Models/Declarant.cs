using System.Text.Json.Serialization;
using TaxManagementAPI.Models;

namespace TaxManagement.Models
{
    public class Declarant: IAuditable
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdateAt { get; set; }
        public string? CreatedByUser { get; set; }
        public string? LastUpdateByUser { get; set; }

        [JsonConstructor]
        public Declarant() {
            Name = String.Empty;
            CreatedByUser = string.Empty;
            LastUpdateByUser = string.Empty;
        }
    }

}
