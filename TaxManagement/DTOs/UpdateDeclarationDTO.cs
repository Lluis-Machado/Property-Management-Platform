using static TaxManagement.Models.Declaration;

namespace TaxManagementAPI.DTOs
{
    public class UpdateDeclarationDTO
    {
        public DeclarationStatus Status { get; set; }
        public DateTime LastUpdateAt { get; set; }
        public string LastUpdateByUser { get; set; } = string.Empty;

    }
}
