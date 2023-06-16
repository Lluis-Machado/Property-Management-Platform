using System.Buffers;

namespace OwnershipAPI.Models
{
    public class OwnershipDTO 
    {
        public Guid Id { get; set; }
        public Guid ContactId { get; set; }
        public Guid PropertyId { get; set; }
        public decimal Share { get; set; } = 0;

        public OwnershipDTO() { }
    }
}
