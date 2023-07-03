using System.Buffers;

namespace OwnershipAPI.Models
{
    public class OwnershipDto 
    {
        public Guid Id { get; set; }
        public Guid ContactId { get; set; }
        public Guid PropertyId { get; set; }
        public decimal Share { get; set; } = 0;
        public bool MainOwnership { get; set; }

        public bool Deleted { get; set; } = false;

        
        public OwnershipDto() { }
    }
}
