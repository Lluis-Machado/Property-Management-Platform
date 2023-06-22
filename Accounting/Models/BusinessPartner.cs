using System.Text.Json.Serialization;

namespace Accounting.Models
{
    public class BusinessPartner
    {
        public string Name { get; set; }
        public string VATNumber { get; set; }
        public string AccountID { get; set; }
        public string Type { get; set; }

        public Guid Id { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
        public string LastModificationByUser { get; set; }

        [JsonConstructor]
        public BusinessPartner()
        {
            Name = string.Empty;
            VATNumber = string.Empty;
            AccountID = string.Empty;
            Type = string.Empty;
            LastModificationByUser = string.Empty;
        }
    }
}
