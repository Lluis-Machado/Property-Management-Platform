using OwnershipAPI.Models;

namespace OwnershipAPI.DTOs
{
    public class OwnershipOperationDto
    {
        public OwnershipDto Values { get; set; }
        public string Operation { get; set; }
    }
}
