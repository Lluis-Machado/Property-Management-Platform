using TaxManagement.Models;

namespace TaxManagementAPI.DTOs
{
    public class DeclarantDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdateAt { get; set; }
        public string CreatedByUser { get; set; } = string.Empty;
        public string LastUpdateByUser { get; set; } = string.Empty;
    }
}
