

namespace AccountingAPI.DTOs
{
    public class TenantDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModificationAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastModificationBy { get; set; }
    }
}
