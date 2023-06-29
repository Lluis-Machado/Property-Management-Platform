using static TaxManagement.Models.Declaration;

namespace TaxManagementAPI.DTOs
{
    public class CreateDeclarationDTO
    {
        public Guid DeclarantId { get; set; }
        public DeclarationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedByUser { get; set; } = string.Empty;
    }
}
