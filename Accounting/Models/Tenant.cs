using AccountingAPI.Models;
using System.Text.Json.Serialization;

namespace AccountingAPI.Models
{
    public class Tenant :BaseModel
    {
        public string? Name { get; set; }

        [JsonConstructor]
        public Tenant()
        {
        }
    }
}
