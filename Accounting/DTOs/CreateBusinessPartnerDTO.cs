using System.Text.Json.Serialization;

namespace AccountingAPI.DTOs
{
    public class CreateBusinessPartnerDTO
    {
        public string? Name { get; set; }
        public string? VATNumber { get; set; }

        [JsonConstructor]
        public CreateBusinessPartnerDTO()
        {
        }
    }
}
