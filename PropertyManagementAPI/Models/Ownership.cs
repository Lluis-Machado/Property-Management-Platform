namespace PropertyManagementAPI.Models
{
    public class xOwnership : BaseModel
    {
        public Guid ContactId { get; set; }
        public Guid PropertyId { get; set; }
        public decimal Share { get; set; }
    }
}
