
namespace AccountingAPI.DTOs
{
    public class CreateBusinessPartnerDTO
    {
        public string Name { get; set; }
        public string VATNumber { get; set; }

        public CreateBusinessPartnerDTO()
        {
            Name = string.Empty;
            VATNumber = string.Empty;
        }
    }
}
