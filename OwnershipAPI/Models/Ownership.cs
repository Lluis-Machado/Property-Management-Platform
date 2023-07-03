namespace OwnershipAPI.Models
{
    public class Ownership : BaseModel
    {
        public Guid ContactId { get; set; }
        public Guid PropertyId { get; set; }
        public decimal Share { get; set; } = 0;
        public bool MainOwnership { get; set; }

        public Ownership() { 

        }
    }
}
