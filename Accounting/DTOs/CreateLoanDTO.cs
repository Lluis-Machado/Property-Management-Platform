using System.Text.Json.Serialization;

namespace AccountingAPI.DTOs
{
    public class CreateLoanDTO
    {
        public string Concept { get; set; }
        public double Amount { get; set; }

        [JsonConstructor]
        public CreateLoanDTO()
        {
            Concept = string.Empty;
        }
    }
}
