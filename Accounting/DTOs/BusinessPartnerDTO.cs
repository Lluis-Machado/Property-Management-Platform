

namespace AccountingAPI.DTOs
{
    public class BusinessPartnerDTO
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; }
        public string VATNumber { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModificationAt { get; set; }
        public string CreatedBy { get; set; }
        public string LastModificationBy { get; set; }

        public BusinessPartnerDTO()
        {
            Name = string.Empty;
            CreatedBy = string.Empty;
            LastModificationBy = string.Empty;
        }

    }
}
