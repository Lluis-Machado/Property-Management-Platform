
namespace AccountingAPI.Models
{
    public class BusinessPartner : BaseModel
    {
        public Guid TenantId { get; set; }
        public string Name { get; set; }
        public string VATNumber { get; set; }

        public BusinessPartner()
        {
            Name = string.Empty;
            VATNumber = string.Empty;
        }

    }
}
