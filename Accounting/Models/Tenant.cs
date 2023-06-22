using System.Text.Json.Serialization;

namespace Accounting.Models
{
    public class Tenant
    {
        public string Name { get; set; }

        public Guid Id { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
        public string LastModificationByUser { get; set; }

        [JsonConstructor]
        public Tenant()
        {
            Name = string.Empty;
            LastModificationByUser = string.Empty;
        }
    }
}
