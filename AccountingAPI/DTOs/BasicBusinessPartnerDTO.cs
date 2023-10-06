

namespace AccountingAPI.DTOs
{
    public class BasicBusinessPartnerDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string VATNumber { get; set; }

        public BasicBusinessPartnerDTO()
        {
            Name = string.Empty;
            VATNumber = string.Empty;
        }

    }
}
