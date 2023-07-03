namespace OwnershipAPI.Models
{
    public class BaseModel
    {
        public Guid? Id { get; set; }
        public Guid TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdateAt { get; set; }
        public string CreatedByUser { get; set; } = string.Empty;
        public string LastUpdateByUser { get; set; } = string.Empty;
        public bool Deleted { get; set; }


    }
}
