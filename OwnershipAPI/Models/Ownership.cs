namespace OwnershipAPI.Models
{
    public class Ownership : BaseModel
    {
        public Guid PropertyId { get; set; }
        public Guid OwnerId { get; set; }
        public string OwnerType { get; set; }
        public decimal Share { get; set; } = 0;
        public bool MainOwnership { get; set; }

        public Ownership() { 

    }
}