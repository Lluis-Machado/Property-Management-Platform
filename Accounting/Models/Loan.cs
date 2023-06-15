using AccountingAPI.Models;
using System.Text.Json.Serialization;

namespace AccountingAPI.Models
{
    public class Loan :BaseModel
    {
        public Guid BusinessPartnerId { get; set; }
        public string Concept { get; set; }
        public double Amount { get; set; }

        [JsonConstructor]
        public Loan()
        {
            Concept = string.Empty;
        }
    }
}
