using ContactsAPI.DTOs;

namespace ContactsAPI.Models
{
    public class ContactDetailsDTO : ContactDTO
    {
        public List<OwnershipInfoDTO>? OwnershipInfo { get; set; } = new List<OwnershipInfoDTO>();
    }


}
