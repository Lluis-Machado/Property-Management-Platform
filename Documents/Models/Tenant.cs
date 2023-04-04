using Newtonsoft.Json;

namespace Documents.Models
{
    public class Tenant
    {
        public string? Name { get; set; }

        [JsonConstructor]
        public Tenant() { }

        public Tenant(string pName) {
            Name = pName;
        }
    }
}
