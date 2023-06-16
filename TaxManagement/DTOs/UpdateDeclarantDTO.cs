using TaxManagement.Models;

namespace TaxManagementAPI.DTOs
{
    public class UpdateDeclarantDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Deleted { get; set; }
    }
}
