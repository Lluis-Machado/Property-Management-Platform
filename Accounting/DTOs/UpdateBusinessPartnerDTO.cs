
namespace AccountingAPI.DTOs
{
    public class UpdateBusinessPartnerDTO
    {
        public string Name { get; set; }
        public string VATNumber { get; set; }

        public UpdateBusinessPartnerDTO()
        {
            Name = string.Empty;
            VATNumber = string.Empty;
        }
    }
}
