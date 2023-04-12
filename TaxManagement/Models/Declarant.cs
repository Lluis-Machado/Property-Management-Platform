using System.Text.Json.Serialization;

namespace TaxManagement.Models
{
    public class Declarant
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }

        [JsonConstructor]
        public Declarant() {
            Name = String.Empty;
        }
    }

}
