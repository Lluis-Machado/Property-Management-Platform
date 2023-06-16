using static TaxManagement.Models.Declaration;
using TaxManagement.Models;

namespace TaxManagementAPI.DTOs
{
    public class UpdateDeclarationDTO
    {
        public Guid Id { get; set; }
        public Guid DeclarantId { get; set; }
        public DeclarationStatus Status { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdateAt { get; set; }
        public string CreatedByUser { get; set; }
        public string LastUpdateByUser { get; set; }

    }
}
