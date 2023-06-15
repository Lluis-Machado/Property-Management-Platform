using AccountingAPI.Models;
using System.Text.Json.Serialization;

namespace AccountingAPI.Models
{
    public class BusinessPartner :BaseModel
    {
        public Guid TenantId { get; set; }
        public string? Name { get; set; }
        public string? VATNumber { get; set; }

        [JsonConstructor]
        public BusinessPartner()
        {
        }
    }
}
